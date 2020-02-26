tool
extends EditorPlugin
var dock
var sav_path_line
var tex_path_line
var valid_file = false
var valid_texture = false
enum mode {new,edit,none}
var actual_mode = mode.none
func _enter_tree():
	dock = preload("res://addons/bit_map_font_creator/main.tscn").instance()
	add_control_to_dock(DOCK_SLOT_LEFT_UL, dock)
	sav_path_line = dock.get_node("sav_path")
	sav_path_line.connect("text_changed",self,"look_for_file")
	sav_path_line.connect("text_entered",self,"look_for_file")
	dock.get_node("tex_path").connect("text_changed",self,"look_for_texture")
	dock.get_node("tex_path").connect("text_entered",self,"look_for_texture")
	dock.get_node("Button").connect("button_down",self,"create")
	look_for_texture(dock.get_node("tex_path").text)
	look_for_file(dock.get_node("sav_path").text)
func _process(delta):
	if valid_file == true && valid_texture == true:
		dock.get_node("Button").disabled = false
	else:
		dock.get_node("Button").disabled = true

func look_for_texture(var text):
	if actual_mode != mode.edit:
		if text != "" && ("res://" in text || "usr://" in text) && (".png" in text || ".jpg" in text || ".jpeg" in text):
			if File.new().file_exists(text):
				var l = load(text)
				if l.get_class() != "StreamTexture":
					dock.get_node("Label2").add_color_override("font_color", Color("ff4646"))
					dock.get_node("Label2").text = "Texture path-Invalid"
					valid_texture = false
				else:
					dock.get_node("Label2").add_color_override("font_color", Color("46ff49"))
					dock.get_node("Label2").text = "Texture path-Valid"
					valid_texture = true
			else:
				dock.get_node("Label2").add_color_override("font_color", Color("ff4646"))
				dock.get_node("Label2").text = "Texture path-Invalid"
				valid_texture = false
		else:
			dock.get_node("Label2").add_color_override("font_color", Color("ff4646"))
			dock.get_node("Label2").text = "Texture path-Invalid"
			valid_texture = false
	elif actual_mode == mode.edit:
		dock.get_node("Label2").add_color_override("font_color", Color("ffb246"))
		dock.get_node("Label2").text = "Texture path-Not required"
		valid_texture = true

func look_for_file(var text):
	if text != "" && ("res://" in text || "usr://" in text) && ".tres" in text:
		if File.new().file_exists(text):
			var l = load(text)
			if l.get_class() == "BitmapFont":
				dock.get_node("Label4").text = "File found, will edit it"
				dock.get_node("Button").text = "Add char"
				dock.get_node("Label4").add_color_override("font_color", Color("ffffff"))
				dock.get_node("tex_path").text = "Not needed"
				dock.get_node("tex_path").editable = false
				dock.get_node("Label").add_color_override("font_color", Color("46ff49"))
				dock.get_node("Label").text = "Save path-Valid"
				actual_mode = mode.edit
				look_for_texture(dock.get_node("tex_path").text)
				valid_file = true
			else:
				dock.get_node("Label4").text = "File found, not bitmap font, Can't edit"
				dock.get_node("Label4").add_color_override("font_color", Color("ff0000"))
				dock.get_node("tex_path").editable = true
				dock.get_node("Label").add_color_override("font_color", Color("ff4646"))
				dock.get_node("Label").text = "Save path-Invalid"
				valid_file = false
		else:
			dock.get_node("Label4").text = "File not found, will create one"
			dock.get_node("Button").text = "Create file"
			dock.get_node("tex_path").editable = true
			dock.get_node("Label4").add_color_override("font_color", Color("ffffff"))
			dock.get_node("Label").add_color_override("font_color", Color("46ff49"))
			dock.get_node("Label").text = "Save path-Valid"
			actual_mode = mode.new
			valid_file = true
	else:
		dock.get_node("Label4").text = "Path/format not valid"
		dock.get_node("Label4").add_color_override("font_color", Color("ff0000"))
		dock.get_node("Label").add_color_override("font_color", Color("ff4646"))
		dock.get_node("Label").text = "Save path-Invalid"
		valid_file = false
		actual_mode = mode.none

func create():
	if actual_mode != mode.none:
		var bf
		var tex
		if actual_mode == mode.new:
			if File.new().file_exists(dock.get_node("tex_path").text):
				bf = BitmapFont.new()
				tex = load(dock.get_node("tex_path").text)
				bf.add_texture(tex)
			else:
				dock.get_node("Label4").text = "Can't load texture"
				dock.get_node("Label4").add_color_override("font_color", Color("ff0000"))
				return
		elif actual_mode == mode.edit:
			bf = load(dock.get_node("sav_path").text)
			tex = bf.get_texture(0)

		for i in range(90-33):
			print("")
			print(i)
			print(dock.get_node("letter_button1/x").value*i)
			bf.add_char(i+33,0,Rect2(dock.get_node("letter_button1/x").value * i,0,dock.get_node("letter_button1/w").value,dock.get_node("letter_button1/h").value))
		ResourceSaver.save(dock.get_node("sav_path").text,bf)
		look_for_file(sav_path_line.text)
