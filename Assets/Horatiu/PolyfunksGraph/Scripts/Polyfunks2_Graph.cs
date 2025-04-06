namespace Polyfunks
{
    using Shapes;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class Polyfunks2_Graph : MonoBehaviour
    {
        private const double PI = Math.PI;
        private const double TAU = Math.PI * 2.0;

        public PolyfunksGraph graph;
        private Polyfunks.Polyfunks2State graphState;

        [Range(1, 8)]
        public int oversample = 1;

        private double _t, _tSpin;
        private double sampleRate;
        private double prevAmp;

        [Header("Visualization")]
        public int linePositions = 200;

        [SerializeField]
        private Polyline _pl;
        public Polyline pl
        {
            get
            {
                if (_pl == null)
                {
                    _pl = GetComponent<Polyline>();
                }
                return _pl;
            }
        }

        public void SetParameter(string polyfunksParameter, double value)
        {

        }

        // latest samples at the end.
        private List<V2d> visualizationData = new List<V2d>();

        [Header("Experimental")]
        public bool forceMono = true;
        public bool limiter = true;

        [Space]
        public bool dcBlocker = true;
        [Tooltip("R between 0 and 1. Adds a bit of lowpass as well as reducing dc offset.")]
        public double dcBlockerR = 0.995;
        private double dc_x, dc_y;

        [Header("Debug")]
        public float panLR;

        private AudioSource _audioSource;
        public AudioSource audioSource
        {
            get
            {
                if (_audioSource == null)
                {
                    _audioSource = GetComponent<AudioSource>();
                }
                return _audioSource;
            }
        }



        private void OnValidate()
        {
            if (graph != null)
            {
                graph.OnValidate();
            }
        }

        private void OnEnable()
        {
            sampleRate = AudioSettings.outputSampleRate;
            if (audioSource != null)
            {
            }
        }

        private void Update()
        {
            Update_GetPan();
            Update_DrawPolyLine();

        }

        private void Update_GetPan()
        {
            panLR = audioSource.panStereo;
        }


        private void Update_DrawPolyLine()
        {
            if (pl == null)
                return;

            Update_PolyLineCount();

            var pIndex = 0;
            // take last positions of the line and draw them in reverse order
            for (int i = visualizationData.Count - 1; i >= 0; i--)
            {
                var v = visualizationData[i % visualizationData.Count];
                // add as many positions as available, and as many as they fit in the line renderer if not enough.
                if (pIndex < pl.Count)
                {
                    pl.SetPointPosition(pIndex, v.ToVector3());
                    pIndex++;
                }
                else
                    break;
            }
        }

        private void Update_PolyLineCount()
        {
            if (pl.Count != linePositions)
            {
                if (pl.Count > linePositions)
                {
                    // do nothing if we have more points in the renderer than we are supposed to.
                    // except set those points to zero...??
                    for (int i = 0; i < pl.Count; i++)
                    {
                        pl.SetPointPosition(i, Vector3.zero);
                        // point thickness is a multiplier of line thickness.
                        // hide the points that are over the limit.
                        pl.SetPointThickness(i, i < linePositions ? 1 : 0); 

                    }
                    return;
                    pl.points = pl.points.Take(linePositions).ToList();
                    pl.meshOutOfDate = true;
                }
                else
                {
                    // add more points if we don't have enough in the renderer
                    while (pl.Count < linePositions)
                        pl.AddPoint(Vector3.zero);
                    pl.meshOutOfDate = true;
                }
            }
        }


        /// <summary>
        /// original formula: https://quod.lib.umich.edu/cgi/p/pod/dod-idx/continuous-order-polygonalwaveform-synthesis.pdf?c=icmc;idno=bbp2372.2016.104;format=pdf
        /// </summary>
        /// <param name="angleRad">angle in radians where we are querying the polygon edge amplitude from the center</param>
        /// <param name="n">n, a.k.a. order - the sides of the polygon, a rational number a/b (int a,b) => 3 creates a triangle, 4 a square, but 3.5 makes a weird incomplete shape in between.</param>
        /// <param name="teeth">wacks out the polygon sides. between 0,1</param>
        /// <returns></returns>
        public double GetPolyAmplitude(double angleRad, double n, double teeth)
        {
            return Math.Cos(PI / n) /
                Math.Cos(TAU / n * ((angleRad * n / TAU) % 1) - PI / n + teeth);
        }

        public V2d GetProjectedPoly(double angleRad, double n, double teeth, double spinRad)
        {
            var amp = GetPolyAmplitude(angleRad, n, teeth);
            var avgAmp = (amp + prevAmp) * 0.5;
            prevAmp = amp;
            return
                // vector amplitude on the x component (using angleRad, not spinRad)
                new V2d((graphState.amplitude * avgAmp), 0)
                // then rotate it using both angle and spin to find final display angle.
                .RotateRadians(angleRad + spinRad);
        }


        private void OnAudioFilterRead(float[] data, int channels)
        {
            var w = 1 / sampleRate;
            var inverseOversample = 1 / (float)oversample;

            var dataLen = data.Length / channels;

            V2d projectedPoly = V2d.zero;

            for (int i = 0; i < dataLen; i++)
            {
                var sum = 0.0;
                for (int j = 0; j < oversample; j++)
                {
                    graph.AudioThread_Tick(w);
                    graphState = graph.GetPolyfunksState(); 

                    // we multiply by 2*PI to be able to use Hz for the freq, spin and amp modulation
                    var w2Pi = w * TAU;
                    var increment_t = w2Pi * graphState.frequency;
                    var inc_tSpin = w2Pi * graphState.spin;
                    var inc_t_oversample = increment_t * inverseOversample;
                    var inc_tSpin_oversample = inc_tSpin * inverseOversample;

                    _t += inc_t_oversample;
                    _tSpin += inc_tSpin_oversample;

                    projectedPoly = GetProjectedPoly(_t, graphState.order, graphState.teeth, _tSpin);
                    sum += projectedPoly.y;
                }
                var v = sum * inverseOversample;

                if (dcBlocker)
                {
                    // apply DC blocker based on this: https://www.dsprelated.com/freebooks/filters/DC_Blocker.html
                    // effectively it reduces offset from the center of the waveform, but at the same time it makes a bit of lowpass.
                    var x = v;
                    var y = x - dc_x + 0.995 * dc_y;
                    dc_x = x;
                    dc_y = y;
                    v = y;
                }

                if (limiter)
                {
                    if (double.IsNaN(v))
                    {
                        v = 0;
                    }

                    // apply the dumb limiter here...?
                    v = LimitSimple(v);
                }

                if (channels == 1)
                {
                    data[i] = (float)v;
                }
                else
                {
                    // left pan is one in the middle, one on the left, and zero on the right
                    data[i * channels + 0] = (float)v * (1f - Mathf.InverseLerp(0, 1, panLR));
                    // right pan is one in the middle, zero on the left, and one on the right.
                    data[i * channels + 1] = (float)v * (1f - Mathf.InverseLerp(0, -1, panLR));

                }

                // extract data for visualization. polyPos is just the last poly calculated for these samples
                var polyPos = projectedPoly;
                visualizationData.Add(polyPos);
                if (visualizationData.Count > linePositions)
                    visualizationData.RemoveAt(0);

            }
        }

        // limit and clamp, and use a specially funked up formula for limiting.
        // use this if u wanna plot: https://www.desmos.com/calculator
        private double Limit(double x)
        {
            var absX = Math.Abs(x);
            absX = MathUtils.Clamp01(absX);
            absX = (1.21 * (absX - Math.Pow(absX, 4) / 4));
            return absX * Math.Sign(x);
            
        }

        private double LimitSimple(double x)
        {
            x = MathUtils.Clamp(x, -1.0, 1.0);
            return x;
        }
    }
}