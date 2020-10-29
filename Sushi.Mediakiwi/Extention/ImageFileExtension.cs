using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data;

public static class ImageFileExtension
{
    /// <summary>
    /// Apply the image properties to a Image webcontrol (set url, toolTip and width/height)
    /// </summary>
    /// <param name="image">The image.</param>
    public static void Apply(this ImageFile inImageFile, System.Web.UI.WebControls.Image image)
    {
        image.ImageUrl = inImageFile.Path;
        image.ToolTip = inImageFile.AlternateText;
        image.Height = inImageFile.Height;
        image.Width = inImageFile.Width;
        image.AlternateText = inImageFile.AlternateText;
        image.Visible = true;
    }
}

