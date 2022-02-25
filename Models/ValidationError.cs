﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColdShineSoft.SmartFileCopier.Models
{
	public enum ValidationError
	{
		Required,
		InvalidFilePath,
		InvalidFileNameCharacter,
		InvalidDirectoryPath,
		BracketMissing,
		InvalidRegularExpression,
		InvalidCsScript,
		InvalidDateTimeFormatString
	}
}