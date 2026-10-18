#!/bin/bash
# Do any necessary project setup.
#
# We need this because YarnSpinner and func_godot are finnicky atm... it ships as a full godot
# project rather than just the addon, so we need to manually symlink the addon
# outta its folder.

PROJECT_PATH=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" &>/dev/null && pwd)
ADDONS_PATH="${PROJECT_PATH}/addons"
ADDONS_SRC_PATH="${PROJECT_PATH}/addons_src"

link_addon() {
    ln -snf "${ADDONS_SRC_PATH}/$1" "${ADDONS_PATH}/$2"
    echo "Linked $2 into addons (from ${ADDONS_SRC_PATH}/$1)"
}

echo "Setting up project..."
echo

link_addon "DoveDraft" "DoveDraft"
link_addon "Flowerwall-CRT-shader-for-Godot/addons/flowerwall_crt" "flowerwall_crt"
link_addon "func_godot_plugin/addons/func_godot" "func_godot"
link_addon "godot_dither_shader/shader" "godot_dither_shader/shader"
link_addon "YarnSpinner-Godot/addons/YarnSpinner-Godot" "YarnSpinner-Godot"
