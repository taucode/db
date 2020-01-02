; CREATE
(defblock :name create :is-top t
	(exact-text :classes word :value "CREATE")
	(alt (block :ref create-table) (block :ref create-index))
	(end)
)

; CREATE TABLE
(defblock :name create-table
	(exact-text :classes word :value "TABLE" :name do-create-table)
	(some-text :classes identifier word :name table-name)
	(punctuation :value "(")
	(block :ref column-def :links table-closing next)
	(punctuation :value "," :links column-def next)
	(block :ref constraint-defs)
	(punctuation :value ")" :name table-closing)
)

; column definition
(defblock :name column-def
	(some-text :classes identifier word :name column-name)
	(some-text :classes identifier word :name type-name)
	(opt 
		(punctuation :value "(")
		(some-int :name precision)
		(opt
			(punctuation :value ",")
			(some-int :name scale)
		)
		(punctuation :value ")")
	)
	(opt
		(alt
			(exact-text :classes word :value "NULL" :name null)
			(seq
				(exact-text :classes word :value "NOT")
				(exact-text :classes word :value "NULL" :name not-null)
			)
		)
	)
	(opt
		(exact-text :classes word :value "PRIMARY")
		(exact-text :classes word :value "KEY" :name inline-primary-key)
	)
	(opt
		(exact-text :classes word :value "AUTOINCREMENT" :name autoincrement)
	)
	(opt
		(exact-text :classes word :value "DEFAULT")
		(alt
			(exact-text :classes word :value "NULL" :name default-null)
			(some-int :name default-integer)
			(some-text :classes string :name default-string)
		)
	)
)

; constraint definitions
(defblock :name constraint-defs
	(exact-text :classes word :value "CONSTRAINT" :name constraint)
	(some-text :classes identifier word :name constraint-name)
	(alt (block :ref primary-key) (block :ref foreign-key))
	(alt
		(punctuation :value "," :links constraint)
		(idle)
	)
)

; PRIMARY KEY
(defblock :name primary-key
	(exact-text :classes word :value "PRIMARY" :name do-primary-key)
	(exact-text :classes word :value "KEY")
	(block :ref pk-columns)
)

; PRIMARY KEY columns
(defblock :name pk-columns
	(punctuation :value "(")
	(some-text :classes identifier word :name pk-column-name)
	(opt
		(alt
			(exact-text :classes word :value "ASC" :name asc)
			(exact-text :classes word :value "DESC" :name desc)
		)
	)
	(alt
		(punctuation :value "," :links pk-column-name)
		(idle)
	)
	(punctuation :value ")")
)

; FOREIGN KEY
(defblock :name foreign-key
	(exact-text :classes word :value "FOREIGN" :name do-foreign-key)
	(exact-text :classes word :value "KEY")
	(block :ref fk-columns)
	(exact-text :classes word :value "REFERENCES")
	(some-text :classes identifier word :name fk-referenced-table-name)
	(block :ref fk-referenced-columns)
)

; FOREIGN KEY columns
(defblock :name fk-columns
	(punctuation :value "(")
	(some-text :classes identifier word :name fk-column-name)
	(alt
		(punctuation :value "," :links fk-column-name)
		(idle)
	)
	(punctuation :value ")")
)

; FOREIGN KEY referenced columns
(defblock :name fk-referenced-columns
	(punctuation :value "(")
	(some-text :classes identifier word :name fk-referenced-column-name)
	(alt
		(punctuation :value "," :links fk-referenced-column-name)
		(idle)
	)
	(punctuation :value ")")
)

; CREATE INDEX
(defblock :name create-index
	(opt (exact-text :classes word :value "UNIQUE" :name do-create-unique-index))
	(exact-text :classes word :value "INDEX" :name do-create-index)
	(some-text :classes identifier word :name index-name)
	(exact-text :classes word :value "ON")
	(some-text :classes identifier word :name index-table-name)
	(punctuation :value "(")
	(some-text :classes identifier word :name index-column-name)
	(opt
		(alt
			(exact-text :classes word :value "ASC" :name index-column-asc)
			(exact-text :classes word :value "DESC" :name index-column-desc))
	)
	(alt
		(punctuation :value "," :links index-column-name)
		(idle)
	)
	(punctuation :value ")")
)
