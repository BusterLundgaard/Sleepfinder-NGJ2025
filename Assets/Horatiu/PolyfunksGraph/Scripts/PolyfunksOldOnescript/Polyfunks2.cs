namespace Polyfunks
{
    using Shapes;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class Polyfunks2 : MonoBehaviour
    {
        private const double PI = Math.PI;
        public float frequency = 420;
        public float frequencySpin = 420;
        public double amplitude = 1f;
        public float order = 3;

        public float teeth = 0;

        [Range(1, 8)]
        public int oversample = 1;

        private double _t, _tSpin;
        private double sampleRate;

        [Header("Visualization")]
        public int linePositions = 200;

        [SerializeField]
        private LineRenderer _lr;
        public LineRenderer lr
        {
            get
            {
                if (_lr == null)
                {
                    _lr = GetComponent<LineRenderer>();
                }
                return _lr;
            }
        }

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


        // latest samples at the end.
        private List<V2d> visualizationData = new List<V2d>();

        public List<PolyFunkAmpModulator> modulators = new List<PolyFunkAmpModulator>();

        [Header("Experimental")]
        public bool forceMono = true;

        private double prev;

        [Header("Debug")]
        public double timeOffset;


        private void OnValidate()
        {
            order = Mathf.Max(3, order);

            modulators.Clear();
            modulators.AddRange(GetComponentsInChildren<PolyFunkAmpModulator>());
        }

        private void OnEnable()
        {
            sampleRate = AudioSettings.outputSampleRate;

            modulators.Clear();
            modulators.AddRange(GetComponentsInChildren<PolyFunkAmpModulator>());
        }

        private void Update()
        {
            Update_DrawPolyLine();
            Update_DrawLine();
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
                var v = visualizationData[i];
                // add as many positions as available, and as many as they fit in the line renderer if not enough.
                if (pIndex < pl.Count)
                    pl.SetPointPosition(pIndex++, v.ToVector3());
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
                    pl.points = pl.points.Take(linePositions).ToList();
                    pl.meshOutOfDate = true;
                }
                else
                {
                    while (pl.Count < linePositions)
                        pl.AddPoint(Vector3.zero);
                    pl.meshOutOfDate = true;
                }
            }
        }

        private void Update_DrawLine()
        {
            if (lr == null)
                return;

            if (lr.positionCount != linePositions)
            {
                lr.positionCount = linePositions;
            }

            var lrIndex = 0;
            // take last positions of the line and draw them in reverse order
            for (int i = visualizationData.Count - 1; i >= 0; i--)
            {
                var v = visualizationData[i];
                // add as many positions as available, and as many as they fit in the line renderer if not enough.
                if (lrIndex < lr.positionCount)
                    lr.SetPosition(lrIndex++, v.ToVector3());
                else
                    break;
            }
        }

        // TODO: change to doubles
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
                Math.Cos(2 * PI / n * (angleRad * n / (2 * PI) % 1) - PI / n + teeth);
        }

        public V2d GetProjectedPoly(double angleRad, double n, double teeth, double spinRad)
        {
            var amp = GetPolyAmplitude(angleRad, n, teeth);
            var avg = (amp + prev) * 0.5;
            prev = amp;
            return
                // vector amplitude on the x component (using angleRad, not spinRad)
                new V2d((amplitude * avg), 0)
                // then rotate it using both angle and spin to find final display angle.
                .RotateRadians(angleRad + spinRad);
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            var wavelength = 1 / sampleRate;
            var increment_t = wavelength * frequency;
            var increment_tSpin = wavelength * frequencySpin;
            var inc_t_oversample = increment_t / oversample;
            var inc_tSpin_oversample = increment_tSpin / oversample;
            var inverseOversample = 1 / (float)oversample;

            var dataLen = data.Length / channels;
            if (forceMono)
            {
                dataLen = data.Length;
                channels = 1;
            }

            V2d projectedPoly = V2d.zero;

            for (int i = 0; i < dataLen; i++)
            {
                var sum = 0.0;
                for (int j = 0; j < oversample; j++)
                {
                    foreach (var m in modulators)
                    {
                        m.AudioUpdate(wavelength, inverseOversample);
                    }

                    _t += inc_t_oversample;
                    _tSpin += inc_tSpin_oversample;

                    projectedPoly = GetProjectedPoly(Angle(_t + timeOffset), order, teeth, Angle(_tSpin + timeOffset));
                    sum += projectedPoly.y;
                }
                var v = sum * inverseOversample;

                // how to LIMIT ??
                var vf = (float)v;
                vf = Mathf.Clamp(vf, -1, 1);

                for (int c = 0; c < channels; c++)
                {
                    // how to make interesting stereo here?


                    data[i * channels + c] = (float)vf;

                }

                // extract data for visualization. polyPos is just the last poly calculated for these samples
                var polyPos = projectedPoly;
                visualizationData.Add(polyPos);
                if (visualizationData.Count > linePositions)
                    visualizationData.RemoveAt(0);

            }
        }

        private double Angle(double t)
        {
            return PI * 2 * t;
        }
    }
}