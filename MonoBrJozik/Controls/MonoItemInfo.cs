﻿using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoBrJozik.Controls
{
    internal class MonoItemInfo
    {
        public Action Action { get; }
        public Texture2D Texture { get; }
        public Texture2D InnerTexture { get; }
        public string Text { get; }

        public MonoItemInfo(Texture2D texture, Texture2D innerTexture, string text, Action action)
        {
            Action = action;
            Texture = texture;
            InnerTexture = innerTexture;
            Text = text;
        }
    }
}
