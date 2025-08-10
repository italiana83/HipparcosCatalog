using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HipparcosCatalog
{
    public class Cluster
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<int> StarsId { get; set; }

        public BoundingBoxRenderer BoundingBoxRenderer;

        public Cluster(string name)
        {
            BoundingBoxRenderer = new BoundingBoxRenderer(new Color4(0.0f, 0.7f, 1.0f, 0.5f));
            BoundingBoxRenderer.Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            BoundingBoxRenderer.Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            StarsId = new List<int>();
            Name = name;
        }

        public void CalcBoundingBox(Vector3 position)
        {
            BoundingBoxRenderer.Min.X = Math.Min(BoundingBoxRenderer.Min.X, position.X);
            BoundingBoxRenderer.Min.Y = Math.Min(BoundingBoxRenderer.Min.Y, position.Y);
            BoundingBoxRenderer.Min.Z = Math.Min(BoundingBoxRenderer.Min.Z, position.Z);

            BoundingBoxRenderer.Max.X = Math.Max(BoundingBoxRenderer.Max.X, position.X);
            BoundingBoxRenderer.Max.Y = Math.Max(BoundingBoxRenderer.Max.Y, position.Y);
            BoundingBoxRenderer.Max.Z = Math.Max(BoundingBoxRenderer.Max.Z, position.Z);
        }
    }
}
