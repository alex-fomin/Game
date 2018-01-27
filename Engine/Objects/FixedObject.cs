﻿using Engine.Objects;

namespace Engine
{
    public class FixedObject : GameObject
    {
        public bool IsPassable { get; set; }

        public Size Size { get; protected set; }

        public virtual int Height => 1;
        
        public FixedObject()
        {
            IsPassable = true;
        }

        public FixedObject(Size size, uint id)
        {
            IsPassable = true;
            Size = size;
            Id = id;
        }
        
        public override string Name => "Fixed objects";

        public override uint GetDrawingCode()
        {
            return Id;
        }
    }

}
