local root = app.fs.joinPath(app.fs.currentPath, "Color Maze")
local art = app.fs.joinPath(root, "Assets", "Art", "Themes")

local CLEAR = Color(0, 0, 0, 0)

local function rgb(hex, alpha)
    local value = tonumber(hex, 16)
    local r = math.floor(value / 65536) % 256
    local g = math.floor(value / 256) % 256
    local b = value % 256
    return Color(r, g, b, alpha or 255)
end

local function blank(width, height)
    local image = Image(width or 32, height or 32, ColorMode.RGB)
    image:clear(CLEAR)
    return image
end

local function rect(image, x0, y0, x1, y1, color)
    for y = y0, y1 do
        for x = x0, x1 do
            image:putPixel(x, y, color)
        end
    end
end

local function hline(image, x0, x1, y, color)
    for x = x0, x1 do
        image:putPixel(x, y, color)
    end
end

local function vline(image, x, y0, y1, color)
    for y = y0, y1 do
        image:putPixel(x, y, color)
    end
end

local function save(image, path, width, height)
    local sprite = Sprite(width or 32, height or 32)
    sprite:newCel(sprite.layers[1], 1, image, Point(0, 0))
    sprite:saveAs(path)
    sprite:close()
end

local function draw_gem(palette)
    local image = blank()
    local rows = {
        {6, 13, 18}, {7, 12, 19}, {8, 11, 20}, {9, 10, 21},
        {10, 9, 22}, {11, 8, 23}, {12, 7, 24}, {13, 6, 25},
        {14, 7, 24}, {15, 8, 23}, {16, 9, 22}, {17, 10, 21},
        {18, 11, 20}, {19, 12, 19}, {20, 13, 18}, {21, 14, 17},
        {22, 15, 16},
    }

    for _, row in ipairs(rows) do
        hline(image, row[2], row[3], row[1], palette.dark)
    end

    for _, row in ipairs(rows) do
        if row[3] - row[2] > 2 then
            hline(image, row[2] + 1, row[3] - 1, row[1], palette.body)
        end
    end

    hline(image, 13, 18, 7, palette.light)
    hline(image, 10, 12, 9, palette.light)
    hline(image, 8, 9, 11, palette.shine)
    vline(image, 12, 9, 18, palette.light)
    vline(image, 16, 8, 21, palette.dark)
    vline(image, 20, 10, 18, palette.shadow)
    image:putPixel(9, 12, palette.shine)
    image:putPixel(10, 13, palette.shine)
    image:putPixel(22, 14, palette.shadow)
    image:putPixel(21, 16, palette.shadow)
    image:putPixel(15, 21, palette.light)
    return image
end

local function draw_ice_block()
    local image = blank()
    local dark = rgb("163F68")
    local edge = rgb("2474A6")
    local body = rgb("71C9E8")
    local light = rgb("C8F6FF")
    local white = rgb("F4FFFF")
    local shade = rgb("45A5CE")

    local spans = {
        {5, 11, 20}, {6, 9, 22}, {7, 8, 23}, {8, 6, 25},
        {9, 6, 25}, {10, 6, 25}, {11, 5, 26}, {12, 5, 26},
        {13, 5, 26}, {14, 5, 26}, {15, 6, 25}, {16, 6, 25},
        {17, 7, 24}, {18, 7, 24}, {19, 8, 23}, {20, 9, 22},
        {21, 10, 21}, {22, 12, 19},
    }

    for _, row in ipairs(spans) do
        hline(image, row[2], row[3], row[1], dark)
    end

    for _, row in ipairs(spans) do
        if row[3] - row[2] > 2 then
            hline(image, row[2] + 1, row[3] - 1, row[1], body)
        end
    end

    hline(image, 12, 19, 6, light)
    hline(image, 9, 12, 8, white)
    hline(image, 7, 9, 10, light)
    vline(image, 10, 10, 15, white)
    vline(image, 11, 13, 18, light)
    vline(image, 20, 9, 16, shade)
    hline(image, 15, 23, 18, shade)
    image:putPixel(8, 9, white)
    image:putPixel(9, 10, white)
    image:putPixel(13, 19, light)
    image:putPixel(18, 20, edge)
    image:putPixel(23, 11, edge)
    return image
end

local function draw_cpu()
    local image = blank()
    local dark = rgb("071A20")
    local board = rgb("12353B")
    local frame = rgb("1F8A83")
    local body = rgb("35C7A5")
    local light = rgb("86FFE0")
    local gold = rgb("D5A83F")
    local trace = rgb("41E4D0")

    for x = 7, 24, 4 do
        vline(image, x, 1, 4, gold)
        vline(image, x, 27, 30, gold)
    end
    for y = 8, 23, 4 do
        hline(image, 1, 4, y, gold)
        hline(image, 27, 30, y, gold)
    end

    rect(image, 4, 4, 27, 27, dark)
    rect(image, 5, 5, 26, 26, frame)
    rect(image, 7, 7, 24, 24, board)
    rect(image, 9, 9, 22, 22, dark)
    rect(image, 10, 10, 21, 21, body)
    rect(image, 12, 12, 19, 19, light)
    rect(image, 13, 13, 18, 18, frame)
    hline(image, 14, 17, 14, trace)
    hline(image, 14, 17, 17, trace)

    for _, point in ipairs({
        {6, 6}, {25, 6}, {6, 25}, {25, 25},
        {5, 15}, {26, 15}, {15, 5}, {15, 26},
    }) do
        image:putPixel(point[1], point[2], light)
    end
    return image
end

local function draw_at_field()
    local image = blank()
    local dark = rgb("260A32")
    local edge = rgb("D63CD8", 235)
    local glow = rgb("FF7BF2", 205)
    local fill = rgb("6C2B8F", 105)
    local hot = rgb("FFD34A", 245)

    local spans = {
        {5, 12, 19}, {6, 8, 23}, {7, 6, 25}, {8, 4, 27},
        {9, 4, 27}, {10, 3, 28}, {11, 3, 28}, {12, 3, 28},
        {13, 3, 28}, {14, 3, 28}, {15, 3, 28}, {16, 3, 28},
        {17, 3, 28}, {18, 4, 27}, {19, 4, 27}, {20, 6, 25},
        {21, 8, 23}, {22, 12, 19},
    }

    for _, row in ipairs(spans) do
        hline(image, row[2], row[3], row[1], fill)
    end
    for _, row in ipairs(spans) do
        image:putPixel(row[2], row[1], edge)
        image:putPixel(row[3], row[1], edge)
    end

    hline(image, 12, 19, 5, glow)
    hline(image, 8, 11, 6, glow)
    hline(image, 22, 25, 21, dark)
    vline(image, 7, 9, 18, edge)
    vline(image, 24, 9, 18, dark)

    local cx, cy = 15.5, 14.5
    for i = 0, 5 do
        local angle = math.rad(i * 60)
        local x = math.floor(cx + math.cos(angle) * 9)
        local y = math.floor(cy + math.sin(angle) * 9)
        image:putPixel(x, y, hot)
    end
    image:putPixel(15, 14, hot)
    image:putPixel(16, 14, hot)
    image:putPixel(15, 15, glow)
    image:putPixel(16, 15, glow)
    return image
end

local function draw_door(palette, mechanical)
    local image = blank()
    rect(image, 3, 2, 28, 30, palette.dark)
    rect(image, 5, 3, 26, 29, palette.frame)
    rect(image, 7, 5, 15, 28, palette.body)
    rect(image, 17, 5, 24, 28, palette.body)
    vline(image, 16, 5, 28, palette.dark)

    hline(image, 8, 14, 7, palette.light)
    hline(image, 18, 23, 7, palette.shadow)
    hline(image, 8, 14, 24, palette.shadow)
    hline(image, 18, 23, 24, palette.light)

    if mechanical then
        rect(image, 12, 12, 19, 18, palette.dark)
        rect(image, 13, 13, 18, 17, palette.accent)
        hline(image, 14, 17, 15, palette.light)
        hline(image, 9, 12, 10, palette.accent)
        hline(image, 19, 22, 10, palette.accent)
        hline(image, 9, 12, 21, palette.accent)
        hline(image, 19, 22, 21, palette.accent)
        image:putPixel(10, 14, palette.light)
        image:putPixel(21, 14, palette.light)
        image:putPixel(10, 19, palette.accent)
        image:putPixel(21, 19, palette.accent)
    else
        rect(image, 13, 13, 18, 18, palette.dark)
        rect(image, 14, 14, 17, 17, palette.light)
        image:putPixel(15, 15, palette.shadow)
        image:putPixel(16, 15, palette.light)
    end

    for _, point in ipairs({
        {6, 4}, {25, 4}, {6, 28}, {25, 28},
    }) do
        image:putPixel(point[1], point[2], palette.light)
    end
    return image
end

local function draw_chest(palette, electronic)
    local image = blank()
    local dark = palette.dark
    local body = palette.body
    local light = palette.light
    local shadow = palette.shadow

    rect(image, 4, 6, 27, 26, dark)
    rect(image, 5, 7, 26, 12, light)
    rect(image, 6, 13, 25, 25, body)
    hline(image, 6, 25, 13, shadow)
    hline(image, 7, 24, 9, body)

    rect(image, 5, 7, 8, 12, shadow)
    rect(image, 23, 7, 26, 12, shadow)
    vline(image, 5, 7, 25, shadow)
    vline(image, 26, 7, 25, shadow)
    hline(image, 4, 27, 25, dark)
    hline(image, 5, 26, 26, shadow)

    rect(image, 13, 12, 18, 20, dark)
    rect(image, 14, 13, 17, 19, light)
    rect(image, 14, 16, 17, 18, shadow)
    image:putPixel(15, 15, light)
    image:putPixel(16, 15, light)

    if electronic then
        local magenta = rgb("F04CDF")
        local cyan = rgb("43E5F0")
        local yellow = rgb("FFD64A")
        local gray = rgb("596278")

        hline(image, 7, 11, 16, cyan)
        hline(image, 20, 24, 16, magenta)
        hline(image, 7, 10, 21, yellow)
        hline(image, 21, 24, 21, cyan)
        vline(image, 8, 16, 20, magenta)
        vline(image, 23, 16, 20, yellow)
        hline(image, 10, 13, 18, gray)
        hline(image, 18, 21, 18, gray)
        image:putPixel(9, 10, cyan)
        image:putPixel(22, 10, magenta)
        image:putPixel(11, 23, yellow)
        image:putPixel(20, 23, cyan)
    else
        vline(image, 9, 7, 25, palette.band)
        vline(image, 22, 7, 25, palette.band)
        image:putPixel(8, 8, light)
        image:putPixel(9, 8, light)
        image:putPixel(22, 8, light)
        image:putPixel(23, 8, light)
        image:putPixel(7, 24, shadow)
        image:putPixel(24, 24, shadow)
    end

    return image
end

local function draw_dark_blue_water()
    local image = blank()
    local deep = rgb("0A2942")
    local base = rgb("124E78")
    local mid = rgb("1E86B4")
    local light = rgb("54C7E6")
    local glow = rgb("9EEBFA")

    rect(image, 0, 0, 31, 31, deep)
    rect(image, 1, 1, 30, 30, base)
    for _, row in ipairs({5, 12, 20, 27}) do
        for x = 1, 30 do
            local y = row + math.floor(math.sin((x + row) / 3) * 2)
            image:putPixel(x, y, mid)
            image:putPixel(x, y + 1, light)
        end
    end
    hline(image, 5, 10, 9, glow)
    hline(image, 20, 26, 16, glow)
    hline(image, 8, 15, 24, light)
    image:putPixel(3, 3, mid)
    image:putPixel(28, 6, mid)
    image:putPixel(6, 29, mid)
    image:putPixel(29, 25, mid)
    return image
end

local function draw_toxic_lava()
    local image = blank()
    local deep = rgb("102718")
    local base = rgb("1B4D28")
    local mid = rgb("2C8A3F")
    local glow = rgb("79D84F")
    local foam = rgb("C8F56A")

    rect(image, 0, 0, 31, 31, deep)
    rect(image, 1, 1, 30, 30, base)

    for _, band in ipairs({
        {5, 3, 28}, {12, 1, 29}, {20, 3, 27}, {27, 1, 28},
    }) do
        local y = band[1]
        for x = band[2], band[3] do
            local wave = y + math.floor(math.sin(x / 2.5) * 1.4)
            image:putPixel(x, wave, mid)
            image:putPixel(x, wave + 1, glow)
        end
    end

    local bubbles = {
        {7, 8, 2}, {22, 9, 3}, {13, 16, 2},
        {25, 22, 2}, {6, 24, 3}, {17, 28, 2},
    }
    for _, bubble in ipairs(bubbles) do
        local cx, cy, radius = bubble[1], bubble[2], bubble[3]
        for y = cy - radius, cy + radius do
            for x = cx - radius, cx + radius do
                local distance = math.sqrt(
                    (x - cx) * (x - cx) + (y - cy) * (y - cy))
                if distance >= radius - 0.5 and
                    distance <= radius + 0.5 then
                    image:putPixel(x, y, glow)
                elseif distance < radius - 1.2 then
                    image:putPixel(x, y, mid)
                end
            end
        end
        image:putPixel(cx - 1, cy - 1, foam)
    end

    hline(image, 2, 5, 3, glow)
    hline(image, 27, 29, 17, foam)
    return image
end

local function save_tile(theme, name, image)
    save(image, app.fs.joinPath(art, theme, name .. ".png"), 32, 32)
end

local sapphire = {
    dark = rgb("142C54"),
    body = rgb("326CC3"),
    light = rgb("77C8FF"),
    shine = rgb("DDF9FF"),
    shadow = rgb("214D95"),
}

local ruby = {
    dark = rgb("5C1027"),
    body = rgb("C72D4B"),
    light = rgb("FF7B87"),
    shine = rgb("FFE6DD"),
    shadow = rgb("8F1934"),
}

local ice_door = {
    dark = rgb("122B4A"),
    frame = rgb("2B6FA5"),
    body = rgb("5EB9DE"),
    light = rgb("C4F4FF"),
    shadow = rgb("2C7DA7"),
}

local fire_door = {
    dark = rgb("431119"),
    frame = rgb("8A2C2D"),
    body = rgb("C94A38"),
    light = rgb("FFB450"),
    shadow = rgb("7F2025"),
}

local cyber_door = {
    dark = rgb("090B1B"),
    frame = rgb("252B4D"),
    body = rgb("3E4C73"),
    light = rgb("F05CFF"),
    shadow = rgb("1C6E86"),
    accent = rgb("48E8F2"),
}

local wood_chest = {
    dark = rgb("432717"),
    body = rgb("9A612E"),
    light = rgb("D19A56"),
    shadow = rgb("67411F"),
    band = rgb("C08A4C"),
}

local ice_chest = {
    dark = rgb("112D46"),
    body = rgb("397FA5"),
    light = rgb("8DD9EE"),
    shadow = rgb("1E567B"),
    band = rgb("B7EFFF"),
}

local fire_chest = {
    dark = rgb("431417"),
    body = rgb("A63A31"),
    light = rgb("F07A4E"),
    shadow = rgb("741F27"),
    band = rgb("FFC15A"),
}

local cyber_chest = {
    dark = rgb("080B12"),
    body = rgb("242A3A"),
    light = rgb("69748C"),
    shadow = rgb("111621"),
}

save(draw_chest(wood_chest, false),
    app.fs.joinPath(art, "..", "Tiles", "tile_chest.png"), 32, 32)
save_tile("deepsea", "tile_material", draw_gem(sapphire))
save_tile("deepsea", "tile_exit", draw_door(ice_door, false))
save_tile("deepsea", "tile_stone", draw_ice_block())
save_tile("deepsea", "tile_chest", draw_chest(ice_chest, false))
save_tile("lava_heart", "tile_material", draw_gem(ruby))
save_tile("lava_heart", "tile_exit", draw_door(fire_door, false))
save_tile("lava_heart", "tile_chest", draw_chest(fire_chest, false))
save_tile("cyberpunk", "tile_material", draw_cpu())
save_tile("cyberpunk", "tile_stone", draw_at_field())
save_tile("cyberpunk", "tile_water", draw_dark_blue_water())
save_tile("cyberpunk", "tile_lava", draw_toxic_lava())
save_tile("cyberpunk", "tile_exit", draw_door(cyber_door, true))
save_tile("cyberpunk", "tile_chest", draw_chest(cyber_chest, true))

local function load_resized(path)
    local image = Image{ fromFile = path }
    image:resize(32, 32)
    return image
end

local function make_preview(theme)
    local folder = app.fs.joinPath(art, theme)
    local preview = blank(64, 64)
    preview:drawImage(load_resized(
        app.fs.joinPath(folder, "tile_wall.png")), Point(0, 0))
    preview:drawImage(load_resized(
        app.fs.joinPath(folder, "tile_floor.png")), Point(32, 0))
    preview:drawImage(load_resized(
        app.fs.joinPath(folder, "tile_floor.png")), Point(0, 32))
    preview:drawImage(load_resized(
        app.fs.joinPath(folder, "tile_stone.png")), Point(0, 32))
    preview:drawImage(load_resized(
        app.fs.joinPath(folder, "tile_floor.png")), Point(32, 32))
    preview:drawImage(load_resized(
        app.fs.joinPath(folder, "tile_chest.png")), Point(32, 32))
    save(preview, app.fs.joinPath(folder, "preview.png"), 64, 64)
end

local function make_classic_preview()
    local folder = app.fs.joinPath(art, "..", "Tiles")
    local preview = blank(64, 64)
    preview:drawImage(load_resized(
        app.fs.joinPath(folder, "tile_wall.png")), Point(0, 0))
    preview:drawImage(load_resized(
        app.fs.joinPath(folder, "tile_floor.png")), Point(32, 0))
    preview:drawImage(load_resized(
        app.fs.joinPath(folder, "tile_floor.png")), Point(0, 32))
    preview:drawImage(load_resized(
        app.fs.joinPath(folder, "tile_stone.png")), Point(0, 32))
    preview:drawImage(load_resized(
        app.fs.joinPath(folder, "tile_floor.png")), Point(32, 32))
    preview:drawImage(load_resized(
        app.fs.joinPath(folder, "tile_chest.png")), Point(32, 32))
    save(preview,
        app.fs.joinPath(folder, "theme_default_preview.png"),
        64, 64)
end

make_preview("deepsea")
make_preview("lava_heart")
make_preview("cyberpunk")
make_classic_preview()

print("Theme assets written to " .. art)
