from sys import argv
from os import listdir
from pathlib import Path


if len(argv) < 4:
    print('Invalid number of arguments')
    exit(1)

PDIR = argv[1]
OUT_FILE = argv[2]
IMAGE_DIRS = []
for i in range(3, len(argv)):
    IMAGE_DIRS += [argv[i]]

TOP = '''
[gd_resource type="Resource" script_class="CardImages" load_steps={} format=3]
[ext_resource type="Script" path="res://v1/CardImage.gd" id="1_v0pgv"]
[ext_resource type="Script" path="res://v1/CardImages.gd" id="1_boc33"]

'''

IMPORT_FORMAT = '[ext_resource type="Texture2D" path="res://{}" id="ImageImport_{}"]\n'

SUB_RESOURCE_FORMAT = '''
[sub_resource type="Resource" id="Resource_Card{}"]
script = ExtResource("1_v0pgv")
card_key = "{}"
image = ExtResource("ImageImport_{}")

'''

ITEM_FORMAT = 'SubResource("Resource_Card{}")'

BOTTOM = '''
[resource]
script = ExtResource("1_boc33")
Images = Array[ExtResource("1_v0pgv")]([{}])
'''

middle1 = ''
middle2 = ''
bottom = []

card_i = 1
for d in IMAGE_DIRS:
    dir = f'{PDIR}/{d}'
    files = listdir(dir)
    for file in files:
        if not file.endswith('.png'):
            continue
        p = Path(file)
        key = p.stem

        # image import
        middle1 += IMPORT_FORMAT.format(f'{d}{file}', card_i)

        # resource
        middle2 += SUB_RESOURCE_FORMAT.format(card_i, key, card_i)

        # bottom
        bottom += [ITEM_FORMAT.format(card_i)]
        card_i += 1

result = TOP.format(card_i * 2 + 10) + '\n' + middle1 + '\n' + middle2 + '\n' + BOTTOM.format(','.join(bottom))
open(OUT_FILE, 'w').write(result)