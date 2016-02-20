using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Problems.Instances.DataAnalysis.Views;

namespace GP4Sim.Trading.Instances.Views
{
    public partial class TradingImportTypeDialog : DataAnalysisImportTypeDialog
    {
        public new TradingImportType ImportType
        {
            get
            {
                return new TradingImportType()
                {
                    Shuffle = ShuffleDataCheckbox.Checked,
                    TrainingPercentage = TrainingTestTrackBar.Value,
                };
            }
        }

        public TradingImportTypeDialog()
        {
            InitializeComponent();
        }

        protected override void CheckAdditionalConstraints(TableFileParser csvParser)
        {
            base.CheckAdditionalConstraints(csvParser);

        }


    }
}

