namespace Bgt.Ocean.Service.ModelViews.Systems
{
    public class SystemGlobalUnitView
    {
        /// <summary>
        /// Gets or sets the guid of a Unit.
        /// </summary>
        public System.Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets the Unit Name.
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or sets the Unit Name.
        /// </summary>
        public string UnitNameAbbrevaition { get; set; }

        /// <summary>
        /// Gets or sets the id of Unit Type.
        /// </summary>
        public System.Guid? SystemGlobalUnitType_Guid { get; set; }

        /// <summary>
        /// Gets or sets the id of text Controls.
        /// With this ids you can get the name of a Unit in different languages.
        /// </summary>
        public System.Guid? SystemDisplayTextControls_Guid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the record is active or not.
        /// </summary>
        public bool FlagDisable { get; set; }

        /// <summary>
        /// Gets or sets the UnitTypeName.
        /// </summary>
        public string UnitTypeName { get; set; }

        /// <summary>
        /// Gets or sets the user that create the record.
        /// </summary>
        public string UserCreated { get; set; }

        /// <summary>
        /// Gets or sets the date when the record was created.
        /// </summary>
        public string DatetimeCreated { get; set; }

        /// <summary>
        /// Gets or sets the universal date when the record was created.
        /// </summary>
        public string UniversalDatetimeCreated { get; set; }

        /// <summary>
        /// Gets or sets the user that modify the record.
        /// </summary>
        public string UserModified { get; set; }

        /// <summary>
        /// Gets or sets the date when the record was modified.
        /// </summary>
        public string DatetimeModified { get; set; }

        /// <summary>
        /// Gets or sets the universal date when the record was modified.
        /// </summary>
        public string UniversalDatetimeModified { get; set; }

        /// <summary>
        /// Gets or sets the UnitID.
        /// </summary>
        public int? UnitID { get; set; }

        /// <summary>
        /// Gets or sets the Unit Name by user languages.
        /// </summary>
        public string UnitNameDisplayText { get; set; }
    }
}
