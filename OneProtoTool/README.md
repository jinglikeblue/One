# OneProtoTool
One框架配套的协议生成工具

#### 使用说明

config.json中的配置
- protoSourceDir 表示协议放置的目录，相对路径，默认为protos
- csharpOutputDir CSharp代码输出的目录，相对路径，默认为csharp
- protocFile ProtoBuf代码生成工具，相对路径，默认为protoc.exe
- copyPath 生成的代码要拷贝到的目录，绝对路径，null表示不用拷贝

> 配置好以后，执行OneProtoTool.exe即可，如果没配置，第一次运行OneProtoTool.exe会自动生成config.json