using Microsoft.ML.Data;

namespace HRApplication_1._0.MlModel
{
    public class DataModelClass
    {
        public class TextData
        {
            [LoadColumn(0)]
            public string Label;

            [LoadColumn(1)]
            public string Text;
        }

        public class TextFeatures
        {
            [VectorType(100)]
            public float[] Features;
        }

        public class MatchingResult
        {
            public int CVIndex { get; set; }
            public double Similarity { get; set; }
        }
        public class Profile
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string CV { get; set; }
        }
    }
}
