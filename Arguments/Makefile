COMPILER=dmcs
OUT_DIR=build

all:	lib demo

LIB_SRC = \
		./ArgumentsLibrary/Argument.cs \
		./ArgumentsLibrary/Arguments.cs \
		./ArgumentsLibrary/Builders/ArgumentBuilder.cs \
		./ArgumentsLibrary/Builders/OptionBuilder.cs \
		./ArgumentsLibrary/Exceptions/ArgumentsException.cs \
		./ArgumentsLibrary/Exceptions/ArgumentsParseException.cs \
		./ArgumentsLibrary/Exceptions/ArgumentsSetupException.cs \
		./ArgumentsLibrary/Option.cs \
		./ArgumentsLibrary/OptionAlias.cs \
		./ArgumentsLibrary/OptionType.cs

lib:
		mkdir -p $(OUT_DIR)/Release
		$(COMPILER) /out:$(OUT_DIR)/Release/Arguments.dll /target:library $(LIB_SRC)

demo:	lib
		mkdir -p $(OUT_DIR)
		$(COMPILER) /out:$(OUT_DIR)/Release/Demo.exe ./ArgumentsProgram/Program.cs -r:$(OUT_DIR)/Release/Arguments.dll

debug:
		mkdir -p $(OUT_DIR)/Debug
		$(COMPILER) -debug /out:$(OUT_DIR)/Debug/Arguments.dll /target:library $(LIB_SRC)
		$(COMPILER) -debug /out:$(OUT_DIR)/Debug/Demo.exe ./ArgumentsProgram/Program.cs -r:$(OUT_DIR)/Debug/Arguments.dll

clean:
		rm -rf ./build
