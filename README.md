# WikiParser

This is a Wiki content parser. You can download the latest WIKI XML file from WIKI yourself. Then use this parser to parse the XML data. And finally, store all parsing results into MySQL.

The program uses multi-threading and operates in full-memory mode. The program automatically uses the maximum system thread count minus 1 as the concurrency thread count. Runtime efficiency: Approximately 20 minutes on a laptop from 2020. The reference configuration of the laptop is: Intel 11700H, Memory 48GB, SSD 20GB. The total data volume is about 3 million records. The XML file size is 15GB.

Below are the basic functionalities of this parser:

1. Splitting WIKI XML files:
   It can help you split a complete WIKI XML file into multiple smaller XML files for concurrent parsing.

2. Parsing WIKI XML data:
   a. Parsing all keywords along with some version attributes (such as the last version date, author, content byte count).
   b. Parsing all topics under each keyword and deduplicating topics using synonym relationships.
   c. Parsing all inter-reference relationships between keywords and establishing bidirectional indexing. Each keyword can obtain information on which other keywords it references and which keywords have referenced it.
   d. Parsing WIKI navigation list pages and establishing a tree-like index. WIKI list pages provide manual coarse classification above keyword reference relationships, especially providing directories for some famous historical figures and events.

3. Write WIKI keywords and search index into the database.
    It writes all parsed content into the MySQL to facilitate developers to develop various applications based on the database.
   
====================================================================

这是一个Wiki内容解析器。你可以自己从WIKI下载最新的WIKI XML文件。 然后使用该解析器对XML数据进行解析。并最终将所有解析结果存入MySQL。


该程序采用多线程，全内存计算。 程序自动使用系统最大线程数-1 作为并发线程数。运行效率：在2020年的笔记本上约需要20分钟。笔记本参考配置为：intel 11700H , Mem 48GB, SSD 20GB。 总数据量约为300万条记录。 XML文件15GB。


以下是该解析器的基本功能：

1. 切割WIKI XML文件。 

    它可以帮助你把一个完整的WIKI XML 文件 切割成多个小XML文件，以便进行并发解析。


2.  WIKI XML 数据解析。

    a. 解析出所有的关键词，以及该关键词的一些版本属性（最后的版本日期，作者，内容字节数等）

    b. 解析关键词下属的所有Topic，并对利用同义词关系对Topic进行去重。

    c. 解析所有 关键词之间的相互引用关系，并建立双向索引。 每个关键词可以得到：它引用了哪些其它关键词，以及当前关键词被哪些其它关键词引用过。
    
    d. 解析WIKI 导航列表页，并建立树状索引。 WIKI列表页提供了在关键词引用关系之上的，人工粗分类。 尤其是对一些著名的历史人物和历史事件提供了目录。

3. WIKI 关键词和检索索引 写入数据库。

   它将所有解析后的内容写入数据库，以方便开发人员在数据库的基础上开发各类应用程序。
