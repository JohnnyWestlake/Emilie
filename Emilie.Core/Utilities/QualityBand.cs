using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emilie.Core
{
    public enum QualityBand
    {
        Undefined = 0,

        /// <summary>
        /// Bleeding edge, untested, theoretical code. May have numerous bugs. 
        /// Design & functionality is likely to change in the future, or possibly 
        /// even entirely removed.
        /// </summary>
        Experimental = 10,

        /// <summary>
        /// In a future build, this class is likely too have significant changes
        /// </summary>
        PlannedRefactoring = 15,

        /// <summary>
        /// Some-what tested code. May have a few rare issues to still iron out
        /// </summary>
        Preview = 20,

        /// <summary>
        /// Code has been through many hard-fought battles and come out on top. 
        /// When applied to a class, certain methods, properties or fields may
        /// have lower quality band values.
        /// </summary>
        Mature = 30,

        /// <summary>
        /// Code that has never been tested
        /// </summary>
        ActiveDevelopment = 60
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class QualityBandAttribute : Attribute
    {
        public QualityBand  QualityBand     { get; }
        public String       Reasoning       { get; }

        public QualityBandAttribute(QualityBand band, string reasoning = null)
        {
            QualityBand = band;
            Reasoning = reasoning;
        }
    }
}
