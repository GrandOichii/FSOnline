using Godot;
using Godot.Collections;
using System;

namespace FSClient.Encoders;

public partial class JsonEncoder : Encoder
{
	public override string Encode(Dictionary dict)
	{
		var result = dict.ToString();
		return result;
	}
}
