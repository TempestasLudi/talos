# Algorithms to be used in the Talos implementations

## Aim

This document lays out a couple of guidelines to implement the Talos system, in order to make it easier to comply with the specifications given in the readme file.
If you want to contribute, do not feel bound by this document. It is merely a guideline, that may help you create a valid implementation. It is by no means a rulebook, containing strict rules about what jou can and cannot do.

## Building the permission tree

The permission tree of a user consists of nodes. Each node represents a resource or group of resources. The process of building the tree consists of repeatedly adding a rule to the (at first) empty tree.
When adding rule, traverse or create the tree, repeatedly taking the node corresponding to the first part of the rule's path and removing that part from the path. Ultimately, when the path is empty, set the permission of the resulting node to the rule's permission.
	
## Nodes of the permission tree
A node of the permission tree can be of different types, each having its own appearance in a permission rule:

| Name           | Notation     | Meaning                                      |
| -------------- | :----------: | -------------------------------------------- |
| Literal node   | `name`       | Matches exactly the specified name.          |
| Variable node  | `[variable]` | Matches the value of the specified variable. |
| Set node       | `{set}`      | Matches any value in the specified set.      |
| Universal node | `*`          | Matches any value.                           |

## Resolving a user's permission to access a resource
### Prerequisites
When a user's permission is checked, the following information is given:

* The path of the resource permission is being checked for.
* The permission trees of the user and his ancestors (the users he inherits from).
* A set of "variables": key-value pairs, the values being strings.
* A set of "sets": key-value pairs, the values being string arrays.

### The algorithm

Start at the root of the permission tree. For the nth node you come across, consider any child node that matches the nth part of the path, using the following prioritization:

* Using the following order: literal node, variable node, set node, universal node. That is, from most specific to least specific.
* The order of specification in the permission rules.
	
If every part of the path has been used, or if there are no more child nodes, check if the permission of the node has been set. If so, return this permission. If not, go one level up and check the next matched node there.
If no applicable permission is found in the tree, perform the algorithm for the parent of the user, if a parent exists.
If the user does not have a parent, permission is denied for the specified resource.
