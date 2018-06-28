# Unit tests for a talos implementation

## Instructions

### Test files
Most of these tests will use a file. These files are provided in the `test-files` directory. Every test suite will need a symbolic link to these files, which should **not** be committed into the version control system.

## Default variables and sets

If no variables are specified for a test, there should no variables be set. The same goes for sets.

## Tests

### Empty

Verify that, if no permissions have been specified, the user "Junior" has no access to the path "/hello".

### Comment

Verify that if the permissions file `normal/comment.talos` has been loaded, the user "Junior" has no access to the path "/hello".

### Common

Load the permissions file `normal/common.talos`. Verify:

| User  | Path                           | Has access |
| ----- | ------------------------------ | ---------- |
| Admin | /                              | Yes        |
| Admin | /home/5                        | Yes        |
| Admin | /tmp/srv/thing                 | Yes        |
| Admin | /home/5/personalsecrets        | No         |
| Admin | /home/5/personalsecrets/things | No         |

### Clear 1

Load the permissions file `normal/common.talos`. Invoke the `clearPermissions` function. Verify:

| User  | Path                           | Has access |
| ----- | ------------------------------ | ---------- |
| Admin | /                              | No         |
| Admin | /home/5                        | No         |
| Admin | /tmp/srv/thing                 | No         |
| Admin | /home/5/personalsecrets        | No         |
| Admin | /home/5/personalsecrets/things | No         |

### Clear 2

Load the permissions file `normal/inheritance1.talos`. Invoke the `clearPermissions` function. Load the permissions file `normal/common.talos`. Verify:

| User  | Path                           | Has access |
| ----- | ------------------------------ | ---------- |
| Admin | /                              | Yes        |
| Admin | /home/5                        | Yes        |
| Admin | /tmp/srv/thing                 | Yes        |
| Admin | /home/5/personalsecrets        | No         |
| Admin | /home/5/personalsecrets/things | No         |

### Inheritance 1

Load the permissions file `normal/inheritance1.talos`. Verify:

| User  | Path  | Has access |
| ----- | ----- | ---------- |
| A     | x     | Yes        |
| A     | x/y   | Yes        |
| A     | x/z/g | Yes        |
| B     | x     | Yes        |
| B     | x/y   | No         | 
| B     | x/z/g | Yes        |

### Inheritance 2

Load the permissions file `normal/inheritance2.talos`. Verify:

| User  | Path | Has access |
| ----- | -----| ---------- |
| A     | x    | Yes        |
| A     | x/y  | No         |
| A     | x/z  | No         |
| B     | x/z  | No         |
| C     | x/y  | Yes        | 
| C     | x/z  | Yes        |
| C     | x/u  | No         |

### Variables

Load the permissions file `normal/variables.talos`.

Use the following variables:

| Name  | Value |
| ----- | ----- |
| sesid | 15    | 

Verify:

| User | Path         | Has access |
| ---- | ------------ | ---------- |
| User | session      | No         |
| User | session/5517 | No         |
| User | session/15   | Yes        |

Use the following variables:

| Name  | Value |
| ----- | ----- |
| sesid | 20    | 

Verify:

| User | Path       | Has access |
| ---- | ---------- | ---------- |
| User | session/20 | Yes        |

### Sets

Use the following sets:

| Name           | Values  |
| -------------- | ------- |
| ownedDevices   | 1, 2, 3 | 
| public         | 4, 5    | 
| allowedDevices | 6, 17   | 

Verify:

| User  | Path               | Has access |
| ----- | ------------------ | ---------- |
| User  | devices            | No         |
| User  | devices/3          | Yes        |
| User  | devices/5          | No         |
| User  | devices/5/control  | Yes        |
| User  | devices/17/        | No         |
| User  | devices/17/control | Yes        |
| User  | devices/7/control  | No         |
| Admin | devices/7          | Yes        |
| Admin | devices/7/control  | Yes        |

### Permission conflict

Load the permissions file `normal/permission-conflict.talos`.

| User | Path  | Has access |
| ---- | ----- | ---------- |
| A    | x/y/z | No         |

### Missing variable

Load the permissions file `normal/missing-set.talos`. Verify that checking whether the user "A" has access to "/a", without any variables, gives an missing value exception.

### Missing set

Load the permissions file `normal/missing-set.talos`. Verify that checking whether the user "A" has access to "/a", without any sets, gives an missing value exception.

### Malformed inheritance 1

Try to load the permissions file `malformed/inheritance1.talos` and verify that that gives a malformed rule exception.

### Malformed inheritance 2

Try to load the permissions file `malformed/inheritance2.talos` and verify that that gives a malformed rule exception.

### Ambiguous rule

Try to load the permissions file `malformed/ambiguous.talos` and verify that that gives a malformed rule exception.

### Malformed permission

Try to load the permissions file `malformed/permission.talos` and verify that that gives a malformed rule exception.
