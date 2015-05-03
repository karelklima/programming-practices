COMPILER=dmcs
OUT_DIR=build
DOXYGEN_VERSION=$(shell doxygen -v 2>/dev/null)
all:	lib doxygen

LIB_SRC = 	ArgumentsLibrary/Argument.cs \
			ArgumentsLibrary/Arguments.cs \
			ArgumentsLibrary/Builders/ArgumentBuilder.cs \
			ArgumentsLibrary/Builders/OptionBuilder.cs \
			ArgumentsLibrary/CommandLine.cs \
			ArgumentsLibrary/Converter.cs \
			ArgumentsLibrary/Exceptions/ArgumentsException.cs \
			ArgumentsLibrary/Exceptions/CommandLineException.cs \
			ArgumentsLibrary/Exceptions/ParseException.cs \
			ArgumentsLibrary/Exceptions/SetupException.cs \
			ArgumentsLibrary/HelpTextGenerator.cs \
			ArgumentsLibrary/Option.cs \
			ArgumentsLibrary/OptionAlias.cs \
			ArgumentsLibrary/OptionType.cs \
			ArgumentsLibrary/Parser.cs \
			ArgumentsLibrary/Properties/AssemblyInfo.cs

TESTS_SRC =	ArgumentsTest/ArgumentsTest.cs \
			ArgumentsTest/Builders/ArgumentBuilderTest.cs \
			ArgumentsTest/Builders/OptionBuilderTest.cs \
			ArgumentsTest/CommandLineTest.cs \
			ArgumentsTest/ConverterTest.cs \
			ArgumentsTest/OptionAliasTest.cs \
			ArgumentsTest/ParserTest.cs


lib:
		mkdir -p $(OUT_DIR)/Release
		$(COMPILER) /out:$(OUT_DIR)/Release/Arguments.dll /target:library $(LIB_SRC)

debug:
		mkdir -p $(OUT_DIR)/Debug
		$(COMPILER) -debug /out:$(OUT_DIR)/Debug/Arguments.dll /target:library $(LIB_SRC)
doxygen:
		if [ -z "$(DOXYGEN_VERSION)" ]; then echo "Doxygen is not installed"; fi;
		if [ -n "$(DOXYGEN_VERSION)" ]; then doxygen Doxygen-config; fi;
test: 	debug
		$(COMPILER) -debug /out:$(OUT_DIR)/Debug/ArgumentsTest.dll /target:library $(TESTS_SRC) -r:$(OUT_DIR)/Debug/Arguments.dll -r:./ArgumentsTest/NUnit-2.6.4/bin/nunit.framework.dll
		cp ./ArgumentsTest/NUnit-2.6.4/bin/nunit.framework.dll $(OUT_DIR)/Debug/nunit.framework.dll
		mono --debug ./ArgumentsTest/NUnit-2.6.4/bin/nunit-console.exe $(OUT_DIR)/Debug/ArgumentsTest.dll

clean:
		rm -rf ./build
		rm -rf ./doc
