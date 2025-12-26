@echo off

::
:: converts templates to generators
::

set _cli=dtomaker

call :t2g JsonSystemText
call :t2g JsonNewtonSoft
call :t2g MsgPack2
call :t2g MemBlocks

goto :eof

:t2g
    call %_cli% t2g -s .\Template.%1\EntityTemplate.cs -o .\DTOMaker.SrcGen.%1\EntityGenerator.g.cs -n DTOMaker.SrcGen.%1
    goto :eof
