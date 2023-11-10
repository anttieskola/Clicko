#region copyright
// Copyright © 2011-2012 Antti Eskola. All rights reserved.
#endregion
namespace AEngine
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Input.Touch;
    using Microsoft.Xna.Framework.Media;
    using Microsoft.Devices;
    #endregion

    /// <summary>
    /// Effects engine
    /// - musics
    /// - soundfx
    /// - tactile feedback
    /// </summary>
    public class AEffects
    {
        #region Private Fields
        private ContentManager _content;

        private static int _MAX_EFFECT = 15;
        private SoundEffectInstance[] _effects;
        private byte _effectIndex;
        private VibrateController _feedBack;
        #endregion

        #region Initialization
        public AEffects(ContentManager content)
        {
            _content = content;
            _effects = new SoundEffectInstance[_MAX_EFFECT];
            _effectIndex = 0;
            _feedBack = VibrateController.Default;
        }
        #endregion

        #region Private helpers
        private byte NextEffectIndex()
        {
            _effectIndex = +1;
            if (_effectIndex >= _MAX_EFFECT)
            {
                _effectIndex = 0;
                return 0;
            }
            return _effectIndex;
        }
        #endregion

        #region Common API
        /// <summary>
        /// Unload all sfx/music (stops them also)
        /// Implemented this way so changing screens
        /// does not reset sfx/music as its commonly shared.
        /// </summary>
        public void UnloadAll()
        {
            Stop(); // stop all before unload
            _content.Unload();
        }
        public void Stop()
        {
            StopSfx();
            StopFeedBack();
        }
        public void StopSfx()
        {
            for (byte i = 0; i < _MAX_EFFECT; i++)
            {
                if (_effects[i] != null)
                {
                    _effects[i].Stop();
                }
            }
        }
        public void StopFeedBack()
        {
            _feedBack.Stop();
        }
        #endregion

        #region SoundFX API
        public void PlaySfx(string name)
        {
            SoundEffect sound = _content.Load<SoundEffect>(name);
            byte effect_index = NextEffectIndex();
            _effects[effect_index] = sound.CreateInstance();
            _effects[effect_index].Play();
        }
        #endregion

        #region Vibrator / Haptic feedback API
        public void Vibration(double milliSecs)
        {
            _feedBack.Start(TimeSpan.FromMilliseconds(milliSecs));
        }
        public void Vibration(TimeSpan timeSpan)
        {
            _feedBack.Start(timeSpan);
        }
        public void VibrationVeryCommonSelect()
        {
            _feedBack.Start(TimeSpan.FromMilliseconds(20));
        }
        public void VibrationCommonSelect()
        {
            _feedBack.Start(TimeSpan.FromMilliseconds(30));
        }
        public void VibrationCommonError()
        {
            _feedBack.Start(TimeSpan.FromMilliseconds(40));
        }
        public void VibrationCommonErrorBig()
        {
            _feedBack.Start(TimeSpan.FromMilliseconds(60));
        }
        #endregion
    }
}
