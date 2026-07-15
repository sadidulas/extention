# Microsoft Security Extension - Icon Generator
# Generates Microsoft-style 4-color square icons

Add-Type -AssemblyName System.Drawing

$sizes = @(16, 32, 48, 128)
$outputDir = Join-Path $PSScriptRoot "icons"

foreach ($size in $sizes) {
    $bmp = New-Object System.Drawing.Bitmap($size, $size)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = 'HighQuality'

    $half = $size / 2
    $quarter = $size / 4
    $gap = [Math]::Max(1, [Math]::Floor($size / 32))

    # Microsoft logo colors
    $red = [System.Drawing.Color]::FromArgb(242, 80, 34)      # #F25022
    $green = [System.Drawing.Color]::FromArgb(127, 186, 0)    # #7FBA00
    $blue = [System.Drawing.Color]::FromArgb(0, 164, 239)     # #00A4EF
    $yellow = [System.Drawing.Color]::FromArgb(255, 185, 0)   # #FFB900
    $darkBg = [System.Drawing.Color]::FromArgb(30, 30, 30)    # Background

    # Fill background
    $g.Clear([System.Drawing.Color]::Transparent)

    # Draw rounded squares for each quadrant
    $margin = $gap * 2
    $quadSize = $half - $margin
    $cornerRadius = [Math]::Max(1, [Math]::Floor($size / 16))

    # Helper to draw rounded rect
    function Draw-RoundedRect {
        param($g, $x, $y, $w, $h, $r, $color)
        $brush = New-Object System.Drawing.SolidBrush($color)
        $path = New-Object System.Drawing.Drawing2D.GraphicsPath

        if ($r -gt 0) {
            $path.AddArc($x, $y, $r * 2, $r * 2, 180, 90)
            $path.AddArc($x + $w - $r * 2, $y, $r * 2, $r * 2, 270, 90)
            $path.AddArc($x + $w - $r * 2, $y + $h - $r * 2, $r * 2, $r * 2, 0, 90)
            $path.AddArc($x, $y + $h - $r * 2, $r * 2, $r * 2, 90, 90)
        } else {
            $path.AddRectangle($x, $y, $w, $h)
        }
        $path.CloseFigure()
        $g.FillPath($brush, $path)
        $brush.Dispose()
        $path.Dispose()
    }

    # Top-left: Red
    Draw-RoundedRect -g $g -x $gap -y $gap -w ($half - $gap) -h ($half - $gap) -r $cornerRadius -color $red

    # Top-right: Green
    Draw-RoundedRect -g $g -x ($half) -y $gap -w ($half - $gap) -h ($half - $gap) -r $cornerRadius -color $green

    # Bottom-left: Blue
    Draw-RoundedRect -g $g -x $gap -y ($half) -w ($half - $gap) -h ($half - $gap) -r $cornerRadius -color $blue

    # Bottom-right: Yellow
    Draw-RoundedRect -g $g -x ($half) -y ($half) -w ($half - $gap) -h ($half - $gap) -r $cornerRadius -color $yellow

    $g.Dispose()

    $path = Join-Path $outputDir "icon${size}.png"
    $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    Write-Host "Generated: $path (${size}x${size})"
}

Write-Host "All icons generated successfully!"
