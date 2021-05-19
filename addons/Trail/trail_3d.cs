namespace Trail
{
    using Godot;

    public abstract class Trail3D : ImmediateGeometry
    {
        public bool Emit
        {
            get
            {
                return (bool)Get("emit");
            }
            set
            {
                Set("emit", value);
            }
        }
    }
}