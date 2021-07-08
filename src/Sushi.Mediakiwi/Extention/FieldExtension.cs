using Sushi.Mediakiwi.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Extention
{
    public static class FieldExtension
    {
        /// <summary>
        /// Returns a new Field based on the published value of the SharedFieldTranslation
        /// </summary>
        /// <param name="inField">The field that has a matching SharedFieldTranslation</param>
        /// <param name="sharedValue">The matching SharedFieldTranslation</param>
        /// <returns></returns>
        public static Field ApplySharedValue(this Field inField, SharedFieldTranslation sharedValue) => ApplySharedValue(inField, sharedValue, false);

        /// <summary>
        /// Returns a new Field based on the value of the SharedFieldTranslation
        /// </summary>
        /// <param name="inField">The field that has a matching SharedFieldTranslation</param>
        /// <param name="sharedValue">The matching SharedFieldTranslation</param>
        /// <param name="useEditValue">Use the non-published value when true, else use the published value</param>
        /// <returns></returns>
        public static Field ApplySharedValue(this Field inField, SharedFieldTranslation sharedValue, bool useEditValue)
        {
            if (sharedValue?.ID > 0)
            {
                if (useEditValue)
                {
                    return new Field(sharedValue.FieldName, sharedValue.ContentTypeID, sharedValue.EditValue);
                }
                else
                {
                    return new Field(sharedValue.FieldName, sharedValue.ContentTypeID, sharedValue.Value);
                }
            }

            return inField;
        }
    }
}
