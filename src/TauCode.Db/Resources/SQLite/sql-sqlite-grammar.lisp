; CREATE
(defblock :name create :is-top t
	(word :value "CREATE")
	(alt (block :ref create-table) (block :ref create-index))
	(end)
)

; CREATE TABLE
(defblock :name create-table
	(word :value "TABLE" :name do-create-table)
	(alt (some-ident :name table-name-ident) (some-word :name table-name-word))
	(symbol :value "(")
	(block :ref column-def :links table-closing next)
	(symbol :value "," :links column-def next)
	(block :ref constraint-defs)
	(symbol :value ")" :name table-closing)
)

; column definition
(defblock :name column-def
	(alt (some-ident :name column-name-ident) (some-word :name column-name-word))
	(alt (some-ident :name type-name-ident) (some-word :name type-name-word))
	(opt 
		(symbol :value "(")
		(some-int :name precision)
		(opt
			(symbol :value ",")
			(some-int :name scale)
		)
		(symbol :value ")")
	)
	(opt
		(alt
			(word :value "NULL" :name null)
			(seq
				(word :value "NOT")
				(word :value "NULL" :name not-null)
			)
		)
	)
	(opt
		(word :value "PRIMARY")
		(word :value "KEY" :name inline-primary-key)
	)
	(opt
		(word :value "AUTOINCREMENT" :name autoincrement)
	)
	(opt
		(word :value "DEFAULT")
		(alt
			(word :value "NULL" :name default-null)
			(some-int :name default-integer)
			(some-string :name default-string)
		)
	)
)

; constraint definitions
(defblock :name constraint-defs
	(word :value "CONSTRAINT" :name constraint)
	(alt (some-ident :name constraint-name-ident) (some-word :name constraint-name-word))
	(alt (block :ref primary-key) (block :ref foreign-key))
	(alt
		(symbol :value "," :links constraint)
		(idle)
	)
)

; PRIMARY KEY
(defblock :name primary-key
	(word :value "PRIMARY" :name do-primary-key)
	(word :value "KEY")
	(block :ref pk-columns)
)

; PRIMARY KEY columns
(defblock :name pk-columns
	(symbol :value "(")
	(alt :name pk-column-name-alternatives (some-ident :name pk-column-name-ident) (some-word :name pk-column-name-word))
	(opt
		(alt
			(word :value "ASC" :name asc)
			(word :value "DESC" :name desc)
		)
	)
	(alt
		(symbol :value "," :links pk-column-name-alternatives)
		(idle)
	)
	(symbol :value ")")
)

; FOREIGN KEY
(defblock :name foreign-key
	(word :value "FOREIGN" :name do-foreign-key)
	(word :value "KEY")
	(block :ref fk-columns)
	(word :value "REFERENCES")
	(alt (some-ident :name fk-referenced-table-name-ident) (some-word :name fk-referenced-table-name-word))
	(block :ref fk-referenced-columns)
)

; FOREIGN KEY columns
(defblock :name fk-columns
	(symbol :value "(")
	(alt :name fk-column-name-alternatives (some-ident :name fk-column-name-ident) (some-word :name fk-column-name-word))
	(alt
		(symbol :value "," :links fk-column-name-alternatives)
		(idle)
	)
	(symbol :value ")")
)

; FOREIGN KEY referenced columns
(defblock :name fk-referenced-columns
	(symbol :value "(")
	(alt :name fk-referenced-column-name-alternatives (some-ident :name fk-referenced-column-name-ident) (some-word :name fk-referenced-column-name-word))
	(alt
		(symbol :value "," :links fk-referenced-column-name-alternatives)
		(idle)
	)
	(symbol :value ")")
)

; CREATE INDEX
(defblock :name create-index
	(opt (word :value "UNIQUE" :name do-create-unique-index))
	(word :value "INDEX" :name do-create-index)
	(alt (some-ident :name index-name-ident) (some-word :name index-name-word))
	(word :value "ON")
	(alt (some-ident :name index-table-name-ident) (some-word :name index-table-name-word))
	(symbol :value "(")
	(alt :name index-column-name-alternatives (some-ident :name index-column-name-ident) (some-word :name index-column-name-word))
	(opt
		(alt
			(word :value "ASC" :name index-column-asc)
			(word :value "DESC" :name index-column-desc))
	)
	(alt
		(symbol :value "," :links index-column-name-alternatives)
		(idle)
	)
	(symbol :value ")")
)
