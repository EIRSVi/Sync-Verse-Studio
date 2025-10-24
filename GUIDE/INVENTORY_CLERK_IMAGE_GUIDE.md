# Inventory Clerk - Product Image Management Guide

## Quick Start

### Viewing Products with Images

1. **Open Product Management**
   - Click "Add New Product" or "Product Management" from dashboard
   - You'll see a table with all products

2. **Product Table Columns**
   - **ID**: Product ID number (for reference)
   - **Image**: Product thumbnail (60x60)
   - **Product Name**: Name of the product
   - **SKU**: Stock Keeping Unit
   - **Category**: Product category
   - **Supplier**: Product supplier
   - **Cost Price**: Purchase cost
   - **Selling Price**: Sale price
   - **Stock**: Current quantity
   - **Status**: Stock status (In Stock / Low Stock)

### Adding Images to Products

#### Method 1: Add Images During Product Creation

1. Click "ADD PRODUCT" button
2. Fill in product details (Name, SKU, Category, etc.)
3. **Save the product first** (images can only be added after saving)
4. Click "📷 Manage Images" button
5. Use the image management interface

#### Method 2: Add Images to Existing Products

1. Click "EDIT" on a product in the table
2. Product details load on the left
3. **Product Images panel** appears on the right
4. Use the image buttons:
   - **📷 Add Images**: Select image files from computer
   - **🌐 Add URL**: Add image from web URL
   - **🗑️ Clear**: Remove all images
   - **⭐ Set Primary**: Choose which image is primary

### Image Management Interface

#### Large Preview Area
- Shows the **primary image** (main product image)
- Click to view full-size
- If no primary set, shows first image

#### Thumbnail Panel
- Shows all product images (up to 4 visible at once)
- Scroll to see more
- Each thumbnail shows:
  - Image number (1, 2, 3, 4...)
  - Star (★) if it's the primary image
  - Green border for primary image

#### Image Operations

**Set as Primary**:
- Click any thumbnail
- That image becomes the primary image
- Green border and star appear

**Delete Image**:
- Right-click on a thumbnail
- Confirm deletion
- Image is removed

**Add Multiple Images**:
- Click "📷 Add Images"
- Select multiple files (Ctrl+Click or Shift+Click)
- All selected images are added
- First image becomes primary automatically

**Add from URL**:
- Click "🌐 Add URL"
- Enter image URL (e.g., https://example.com/image.jpg)
- Click "Add"
- Image is downloaded and added

### Supported Image Formats

✅ **Supported**:
- JPG / JPEG
- PNG
- BMP
- GIF
- TIFF
- WebP
- ICO

❌ **Not Supported**:
- PDF
- SVG
- RAW formats

### Best Practices

#### Image Quality
- Use clear, well-lit photos
- Recommended size: 800x800 pixels or larger
- Keep file size under 5MB for best performance

#### Image Organization
- Set a clear primary image (main product view)
- Add multiple angles if available
- Use consistent backgrounds
- Name files descriptively before uploading

#### Primary Image Selection
- Choose the most representative image
- Should show the product clearly
- This image appears in:
  - Product table
  - Sales interface
  - Reports
  - Receipts

### Troubleshooting

#### Images Not Showing?

**Check 1: File Location**
- Images must be in `assets/images/` folder
- Check if files were copied correctly

**Check 2: File Format**
- Ensure file is a supported image format
- Check file extension

**Check 3: File Permissions**
- Ensure you have read access to image files
- Check folder permissions

**Check 4: Image Path**
- Verify path in database is correct
- Should be like: `assets/images/product.jpg`

#### Error Messages

**"Unsupported image type"**
- File format not supported
- Convert to JPG or PNG

**"Error loading image"**
- File may be corrupted
- Try re-uploading
- Check file permissions

**"Image not found"**
- File was deleted or moved
- Re-upload the image

**"No primary image"**
- No images added yet
- Click "Add Images" to add

### Tips & Tricks

#### Quick Product Lookup
- Use the search box to find products by:
  - Product ID
  - Product name
  - SKU
  - Barcode

#### Bulk Operations
- To add images to multiple products:
  1. Edit first product
  2. Add images
  3. Save
  4. Edit next product
  5. Repeat

#### Image Reuse
- If multiple products use same image:
  - Add image to first product
  - Copy image file name
  - Use "Add URL" with local path for others

#### Keyboard Shortcuts
- **Enter**: Save product
- **Esc**: Cancel editing
- **Ctrl+Click**: Select multiple images
- **Right-Click**: Delete thumbnail

### Common Workflows

#### Workflow 1: New Product with Images
1. Click "ADD PRODUCT"
2. Enter product details
3. Click "Save"
4. Click "📷 Manage Images"
5. Add images
6. Set primary image
7. Close form

#### Workflow 2: Update Product Images
1. Find product in table
2. Click "EDIT"
3. Images load automatically
4. Add/remove/reorder as needed
5. Click "Save"

#### Workflow 3: Change Primary Image
1. Edit product
2. Click desired thumbnail
3. Green border appears
4. Click "Save"

#### Workflow 4: Remove All Images
1. Edit product
2. Click "🗑️ Clear"
3. Confirm deletion
4. Click "Save"

### Dashboard Quick Actions

From the Inventory Clerk Dashboard, you can:

- **Add Product Images**: Bulk add images to products without images
- **Add Specific Image**: Add image to specific product by ID
- **View Product Images**: See summary of all product images

### Product ID Reference

- Product IDs are shown in the first column of the table
- Click on a product in Recent Activity to copy its ID
- Click on a product in Low Stock Alerts to copy its ID
- Use these IDs with "Add Specific Image" function

### Image Storage

- Images are stored in: `assets/images/`
- Relative paths saved in database
- Original files are copied (not moved)
- Duplicate names get numbered suffix

### Performance Tips

- Keep image files under 5MB
- Use JPG for photos (smaller file size)
- Use PNG for graphics with transparency
- Resize large images before uploading
- Don't add too many images per product (4-6 is ideal)

---

## Need Help?

If images aren't working:
1. Check the Debug Console for error messages
2. Verify file paths and permissions
3. Ensure images are in correct folder
4. Contact system administrator if issues persist

**Remember**: Always save the product before adding images!
