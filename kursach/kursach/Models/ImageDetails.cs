using System;
public class ImageDetails
{
	/// <summary>
	/// A name for the image, not the file name.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// A description for the image.
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// Full path such as c:\path\to\image.png
	/// </summary>
	public string Path { get; set; }

	/// <summary>
	/// The image file name such as image.png
	/// </summary>
	public string FileName { get; set; }

	/// <summary>
	/// The file name extension: bmp, gif, jpg, png, tiff, etc...
	/// </summary>
	public string Extension { get; set; }

	/// <summary>
	/// The image height
	/// </summary>
	public int Height { get; set; }

	/// <summary>
	/// The image width.
	/// </summary>
	public int Width { get; set; }

	/// <summary>
	/// Overrided Equals method.
	/// </summary>
	/// <param name="other">Other object.</param>
	/// <returns>The result.</returns>
	public bool Equals(ImageDetails other)
	{
		return this.Name == other.Name &&
			this.Description == other.Description &&
			this.Path == other.Path &&
			this.FileName == other.FileName &&
			this.Extension == other.Extension &&
			this.Height == other.Height &&
			this.Width == other.Width;
	}
}
