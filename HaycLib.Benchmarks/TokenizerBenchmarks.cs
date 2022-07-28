using BenchmarkDotNet.Attributes;
using HaycLib.Lexing;

namespace HaycLib.Benchmarks;

[MemoryDiagnoser]
public class TokenizerBenchmarks
{
    [Benchmark]
    public void BasicFunction()
    {
        const string src = @"
            function main() : int32 {
                std::String& myPrintString = ""Hello, world!"";
                std::out().writeln(myPrintString);

                std::out().writeln(""Hello, World!"");
                return 0;
            }
        ";

        Tokenizer tokenizer = new("benchmark", src);
        tokenizer.Tokenize();
    }

    [Benchmark]
    public void StructWithFunc()
    {
        const string src = @"
            struct Point {
			    int32 x;
				int32 y;
				int32 z;

				init(int32 length) {
					this.length = length;
				}
			}

			function @Point print() : void {
				std::out().writeln(this.x, this.y, this.z);
			}
        ";

        Tokenizer tokenizer = new("benchmark", src);
        tokenizer.Tokenize();
    }
}