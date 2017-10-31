# Talos
A multi level authorization project

## The concept
The authorization in this project is an attempt to conveniently define what users can access what resources. Resources are uniquely identified using a "resource identifier", which is a string, possibly consisting of multiple levels separated by slashes in order to keep them well-organized. The users in the systeam each have a role, which also can inherit permissions of another role. The permissions then can be specified using so-called permission rules. For example: 

```
Admin > User

allow User  /
deny  User  /profile
allow User  /profile/[id]
allow Admin /profile
deny  Admin /profile/*/password
allow Admin /profile/[id]/password
```

These are the access rules for a website where people have a profile and everyone can access their own profile, but administrators can also access other people's profiles, except for the password sections.

## Examples
Here are some examples of valid sets of access rules.

### Multiple inheritances
```
B > A
C > B

allow A a
deny  A a/*
allow B a/b
allow C a/c
```
In this case, A has access to a, B has access to both a and a/b and C has access to a, a/b and a/c.

### Sets
```
Admin > User

allow User  devices
deny  User  devices/*
allow User  devices/{ownedDevices}
allow User  devices/{public}/control
allow User  devices/{allowedDevices}/control
allow Admin devices
```
In this case, a user has access to every device he owns and can control any device to which its owner has granted him access. The administrators however, have access to every device regardless of ownership.

### Overriding inheritance
```
B > A

allow A a
allow A a/b
deny  B a
```
Although it has been specified that every A has access to a and a/b, since B does not have access to a, this overrides the aforementioned access to a and any lower level.

### Multiple matching rules
```
allow A a/*/c
deny  A a/b/*
```
Both rules match the resource identifier a/b/c, but the identifiers matched by the first rule lie further apart than the identifiers matched by the second rule. Therefore, the second rule is more specific and will be applied, so A does not have access to a/b/c.
