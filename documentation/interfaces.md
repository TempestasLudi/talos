# The functions that a talos implementation should have

## Types of implementations
Different programming languages express different ideas about what a program should do and how it should work. Therefore, it is only logical that in some cases these specs will not be implemented perfectly. For example, in functional programming languages, it might be necessary for functions to return a modified object, instead of modifying an object itself. The aim of this specification is only to achieve some level of consistency in the function of the different implementations. Whenever you diverge in an implementation, add a comment about that to its documentation.

The function and variable names in the text below probably have to be altered slightly in order to match the naming convention in a particular language. Please follow naming conventions! It makes reading code so much less painful.

## The core
The core of the system is determining who has access to what. To that end, suppose the permission rules have already been loaded. Then, the following function should be available:

```
\texttt{hasAccess(role, path, variables, sets)}
```

This function calculates whether the specified role has access to the resource indicated by \texttt{path}, given the specified variables and sets. It returns a boolean. The types of the arguments are:

| *Name*      | *Type*                                                                     |
| ----------- | -------------------------------------------------------------------------- |
| `role`      | string                                                                     |
| `path`      | string                                                                     |
| `variables` | set of key-value pairs, with both keys and values being strings            |
| `sets`      | set of key-value pairs, the keys being strings, the values sets of strings |
	
## IO
### Basic functionality
The system should at least support the following three methods:

1. ```loadPermissions(permissions)```

	This function loads the permission and inheritance rules specified in `permissions`. It can, if deemed useful by the implementer, construct the permission tree for every role, although this can also be done when checking access for a specific role. The function does not return a value. `permissions` is a string.

2. ```loadPermissionsFile(filename)```

	This function opens the file `filename` and loads the permission and inheritance rules specified therein. It can, if deemed useful by the implementer, construct the permission tree for every role, although this can also be done when checking access for a specific role. The function does not return a value. `filename` is a string, or another useful description of a file.

3. ```clearPermissions()```

	This function unloads the permissions that have previously been loaded.

### Optional functionality
Usually when systems grow, more and more data is moved to a database. The permission and inheritance rules could be put in a database table. The following function can be added:

```loadPermissionsDatabase(connection, [type], names)```

This function loads the permission and inheritance rules from two tables, via `connection`, specified in `names`. The argument `type` is optional to implement. If necessary, it can carry the database type. The function does not return a value. The types of the arguments are:

| *Name*       | *Type*              |
| ------------ | ------------------- |
| `connection` | database connection |
| `type`       | enumerated type     |
| `names`      | key-value object containing the names of the permission and inheritance tables and the names of the relevant columns in those tables, i.e. the columns containing the names of the inheriting role and inherited role for the inheritance rules and the permission (allow/deny), role name and path expression for the permission rules |

