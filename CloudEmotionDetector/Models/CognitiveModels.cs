using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudDetector.Models
{
    public class FaceRectangle
    {
        public int left { get; set; }
        public int top { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Emotion
    {
        public FaceRectangle faceRectangle { get; set; }
        public IDictionary<string, double> scores { get; set; }
    }

    public class OutputEmotion
    {
        public FaceRectangle faceRectangle { get; set; }

        public string primaryEmotion { get; set; }
    }
}
