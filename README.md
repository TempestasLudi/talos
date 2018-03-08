# Talos
A "file system" permission project

## The concept
This project aims to provide a convenient description mechanism for permissions of a file system in its broadest sense: "everything is a file". Any collection of resources can be modeled as a file system.
Resources are dentified using a string (path) consisting of one or multiple levels separated by slashes. Users of the systeam each have a role which can inherit permissions of other roles. Permissions are specified using so-called permission rules, which are triplets of a boolean (allow or deny), the name of a role and the path of a resource. For example:


```
Student > Mara
Student > Jeffrey

allow Admin   /
allow Student /home/[id]
allow Mara    /srv/nfs/music
deny  Jeffrey /home/[id]/config
deny  Admin   /home/*/personalsecrets
```

## The tools

### Inheritance
```
A > B
B > C

allow A x
deny  A x/*
allow B x/y
allow C x/z
```
`A` has access to `x`, `B` has access to both `x` and `x/y` and `C` has access to `x`, `x/y` and `x/z`.

```
A > B

allow A x/*
deny  B x/y
```
Although `A` has access to `x/y`, `B` does not have access to `x/y` as inherited access is overridden.

### Variables
```
deny User session
allow User session/[sesid]
```
This allows a user access to a session, provided the session id matches the variable `sesid`.
Variables, like sets (see the section about sets below), are passed as key-value pairs to the matcher any time the permissions of a user to a resource are determined.

### Sets
```
User > Admin

deny  User  devices/*
allow User  devices/{ownedDevices}
allow User  devices/{public}/control
allow User  devices/{allowedDevices}/control

allow Admin devices
```
A user has access to every device he owns and can control any device to which its owner has granted him access. The administrators however, have access to every device regardless of ownership.

### Multiple matching rules
```
allow A x/*/z
deny  A x/y/*
```
Both rules match the resource identifier `x/y/z`. The identifier most specific on the highest level of difference takes precedence. Therefore, the second rule is more specific and `A` does not have access to `x/y/z`.
