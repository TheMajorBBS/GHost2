GHost/2 Door Server
===================

## Description
A door server built to replace the functionality of the original Major/WorldGroup BBS GHost system.  
But GHost/2 will lend itself to any BBS that has the ability to use RLogin.
GHost/2 is derived from Rick Parrish's [GameSrv](https://github.com/rickparrish/GameSrv)


## Building
If you plan to build from source you will also need the [RMLibUI](https://github.com/rickparrish/RMLibUI) library.
This current project is built in Visual Studio 2019.


## Installing


## Configuring


Release Information
===================

This is a basics on how to understand our versions and what they mean to you.  Our public version covers the entire 
package and consists of a 3 point version system.
The first point is called the major revision and will only be incremented when a migration is going to be required.
You maybe required running a one time migration tool or a fundamental change happens internally that prevents you from
rolling back to a older version.
The second point is the minor revision.  This just gets incremented with every planned release that does not require
any migration.  This includes batches of features and planned bug fixes.
The third point is the hotfix and is only issued when a planned release has a critical bug that requires an immediate fix.
This hotfix can include multiple fixes if necessary but all of the bugs were fast tracked through the project.
For example a critical bug requires a hotfix release.  This might also include less critical fixes to be issued that
were ready to release as-is.

We use 3 stage life cycles on releases.
 - Alpha: This is rough and being planned.  Things will radically change and get scrubbed in this phase.  Code contains
 prototyping and experiments.
 - Beta or Release Candidates: These features are fixed and should make it to a release.  Can contain unoptimized code.
 - Release: The version has been tested, stabilized and optimized.  Unless your a developer or wanting to test, this is the
 life cycle you want to download.

When reporting bugs it's critical you give us the version as well as life cycle your running and detailed steps of how to reproduce
the bug.  Any bug ticket missing this information is subject to being deleted with no notice.  We apologize but it's the only
means of removing noise from the ticket board and prevents developers from wasting time.


Support Website
===============


License
=======

	GHost/2 is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	any later version.

	GHost/2 is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with GHost/2.  If not, see <http://www.gnu.org/licenses/>.
	

Acknowledgements
================

Rick Parrish - Original [GameSrv](https://github.com/rickparrish/GameSrv), [RMLib](https://github.com/rickparrish/RMLib) and [RMLibUI](https://github.com/rickparrish/RMLibUI) 
