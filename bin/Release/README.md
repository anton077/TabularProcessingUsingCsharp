# tabularprocessing
Incremental Processing in Tabular Using Analysis Management Objects (AMO)


The main steps of script:
1.	Get connection strings to Tabular server, DWH server, Tabular DB and list of tables that need refreshing from XML configuration file. 
2.	Connect to DWH DB and get range of data from STG schema for each table in list (date data from, date data to)
3.	Connect to Tabular DB and extract below attributes using RegEx from partition query of partitions of each table in list
a.	The column the table partitioned by
b.	The range of each partition (tables supposed to be partitioned by date type column using signs “<”, “<=”,”>”,”>=”)
4.	Compare partition range with updated data range. If data range overlaps partition range – process it!




  
  
