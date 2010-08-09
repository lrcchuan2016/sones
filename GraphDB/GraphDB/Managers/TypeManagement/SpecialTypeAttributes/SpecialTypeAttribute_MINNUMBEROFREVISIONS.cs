﻿#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement.BasicTypes;

#endregion

namespace sones.GraphDB.TypeManagement.SpecialTypeAttributes
{
    public class SpecialTypeAttribute_MINNUMBEROFREVISIONS : ASpecialTypeAttribute
    {

        #region AttributeUUID

        public static AttributeUUID AttributeUUID = new AttributeUUID(16);

        #endregion

        #region Name

        public static String AttributeName = "MIN_NUMBER_OF_REVISIONS";

        #endregion

        #region Constructors

        public SpecialTypeAttribute_MINNUMBEROFREVISIONS()
        {
            Name = AttributeName;
            UUID = AttributeUUID;
        }

        #endregion

        public override string ShowSettingName
        {
            get
            {
                return "MIN_NUMBER_OF_REVISIONS";
            }
        }

        public override Exceptional ApplyTo(DBObjectStream myNewDBObject, object myValue, params object[] myOptionalParameters)
        {
            return new Exceptional(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
        }

        public override Exceptional<AObject> ExtractValue(DBObjectStream dbObjectStream, GraphDBType graphDBType, DBContext dbContext)
        {
            return new Exceptional<AObject>(new DBUInt64(dbObjectStream.MinNumberOfRevisions));
        }

    }
}