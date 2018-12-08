# DestroyScript

A simple compile to make game dialogue chain

# Version 1.0

<font color=#0099ff>DestroyScript 包含:</font>
  > 1. DestroyScript 编译器
  > 2. DestroyScript 动态链接库

<font color=#9933ff>DestroyScript 编译器特性</font>

1. DestroyScript编译器会自动识别源代码的编码格式并转换为UTF-8格式。
   > UTF-8
2. 在DestroyScript源代码中只识别英文的符号。
   > Limited to English Symbols

3. 编译器根据回车拆分行。
   > Enter

3. 以下几种行会被编译器忽略
   > 空白行(没有任何字符)

   > 注释行(以#符号开头)
   
   > 错误行(不包含[ ] 或 只包含[ ])

3. DestroyScript源代码最多支持100列, int.MaxValue行。
   > Limited to Column(100), DataLine(int.MaxValue)

4. 一个数据表不能没有行, 只识别一行标识行与类型行(至少有一行标识行与类型行, 可以没有数据行)。
   > 1 DataLine 1 TypeLine

<font color=#9933ff>DestroyScript 源代码语法(基于符号)</font>

1. 在DestroyScript源代码的第一行的开头使用 ! 符号声明当前行为设置行。可以使用以下关键字。(如果类名以数字开头会自动在前面加上下划线_)
   - gencs     生成.cs文件
   - genid  生成Id列
   - class(类名) 指定文件类名
   > !gencs
   
   > ! gencs

   > ! gencs, genid , class(DestroyScript)

2. 在任意一行的开头使用 # 符号可以注释该行, 该行会被编译器忽略。
   > #我是注释, 你看不到我！

1. 使用 [ ] 符号在一行中创建一个列的容器, 列可以写在[ ]之间。这对中括号必须一个写在该行的开头, 另一个写在该行的结尾。
   > [ ]

2. 使用 @ 符号声明标识列, 标识必须紧跟 @ 符号。
   > [@name]

4. 使用 ~ 符号声明类型列, 类型必须紧跟 ~ 符号。
   - string 字符串类型
   - int    整数类型
   - float  浮点数类型
   - bool   布尔类型
   > [~string]

3. 使用 # 符号声明数值列, 数值必须紧跟 # 符号。
   > [#老王]

5. 使用 , 符号分割不同列, 使用逗号分隔的列两边可以使用任意空格, 以下形式都被智能的编译器允许。
   > [@name,@age,@address]

   > [@name, @age, @address]

   > [ @name , @age , @address ]

7. 换行必须使用 回车键 。只要不按回车键, 可以一直编辑, 即使该行已经超出了文本编辑器一行的宽度, 依然被编译器视作同一行。

<font color=#77ffaa>DestroyScript 编译器</font>
1. 编译器信息
   > DestroyScript.exe

   > based on .Net Framework 4.6.1

2. 输入同目录下的.txt文件名, 按下回车开始编译。

<font color=#8899ff>DestroyScript 动态链接库</font>
1. 动态链接库信息
   > DestroyScript.dll

   > based on .Net Framework 4.6.1

2. 在C#项目中导入DestroyScript.dll API

3. 熟悉内置类型
   > - <font color=#9977>DataTable</font>
   > - <font color=#6699>DataLine</font>

4. 使用<font color=#6699> DestroyScript.DestroyScriptHelper </font>提供的API
   > 1. 获取DLL路径下的DestroyScript数据表, 失败则返回空。
   >>> <font color=#9999>GetDataTable(string fileName)</font>
   > 2. 获取指定路径下的DestroyScript数据表，失败则返回空。
   >>> <font color=#9999>GetDataTableDirect(string absolutePath)</font>
   > 3. 获取指定文本的DestroyScript数据表，失败则返回空。
   >>> <font color=#9999>GetDataTableByText(string text)</font>

   > 1. 反序列化DestroyScript数据行到指定类型的对象，失败则返回空。
   >>> <font color=#9999>Deserialize(DataLine idenLine, DataLine typeLine, DataLine valueLine)</font>
   > 2. 反序列化DLL路径下的DestroyScript文件到指定类型的对象，失败则返回空。
   >>> <font color=#9999>Deserialize(string fileName)</font>
   > 3. 反序列化DestroyScript源代码到指定类型的对象, 失败则返回空。
   >>> <font color=#6699>DeserializeByText(string text)</font>

   > 1. 序列化一个对象到DestroyScript源代码, 失败则返回空。
   >>> <font color=#6699>Serialize(T instance)</font>
   > 2. 序列化一个对象到DestroyScript源代码, 失败则返回空。
   >>> <font color=#6699>Serialize(List<T> list)</font>

   > <font color="red">注意:只会识别该类中的string, int, float, bool四种类型</font>
   > 1. 序列化一个对象到byte[], 失败则返回空。
   >>> <font color=#6699>Serialize2Bit(T instance)</font>
   > 2. 反序列化一个对象到byte[], 失败则返回空。
   >>> <font color=#6699>Deserialize4Bit(byte[] data)</font>