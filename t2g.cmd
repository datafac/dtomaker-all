@echo off

::
:: converts templates to generators
::

set _cli=dtomaker

call :t2g JsonSystemText

goto :eof

:t2g
    call %_cli% t2g -s .\Template.%1\EntityTemplate.cs -o .\DTOMaker.SrcGen.%1\EntityGenerator.g.cs -n DTOMaker.SrcGen.%1
    goto :eof
