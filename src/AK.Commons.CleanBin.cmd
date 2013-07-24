@echo off
for /f "tokens=*" %%f in ('dir /s /ad /b bin;obj') do rd /s /q "%%f"
for /f "tokens=*" %%f in ('dir /s /ad /b ..\lib') do rd /s /q "%%f"
