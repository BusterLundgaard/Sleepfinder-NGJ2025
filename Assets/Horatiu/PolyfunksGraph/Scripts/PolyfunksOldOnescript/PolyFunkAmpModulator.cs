namespace Polyfunks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;


    public class PolyFunkAmpModulator : MonoBehaviour
    {
        private const double PI = Math.PI;

        private Polyfunks2 _polyfunks;
        public Polyfunks2 polyfunks
        {
            get
            {
                if (_polyfunks == null)
                {
                    _polyfunks = GetComponent<Polyfunks2>();
                }
                return _polyfunks;
            }
        }

        public bool active = true;
        public double ampMax = 0.25;
        public float frequency = 420;

        private double _t;

        public double GetAmplitude(double t)
        {
            var modAmp = (ampMax * (Math.Sin(t) * 0.5 + 0.5));

            return active ? modAmp : ampMax;
        }

        private void OnEnable()
        {
            if (polyfunks != null)
            {
            }
        }

        // called from audio thread on main Polyfunks
        public void AudioUpdate(double wavelength, float inverseOversample)
        {
            _t += wavelength * frequency * inverseOversample;
            polyfunks.amplitude = GetAmplitude(_t);
        }
    }
}