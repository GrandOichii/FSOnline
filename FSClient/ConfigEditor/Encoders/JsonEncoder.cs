using Godot;
using Godot.Collections;
using System;

namespace FSClient.Encoders;

public partial class JsonEncoder : Encoder
{
	public override string Encode(Dictionary dict)
	{
		return dict.ToString();
		// return JsonSerializer.Serialize(dict);
	}
}
