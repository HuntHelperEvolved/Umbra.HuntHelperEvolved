"""Package only the companion and its documentation from an existing Release build."""
import hashlib
from pathlib import Path
from xml.etree import ElementTree
from zipfile import ZipFile, ZipInfo, ZIP_DEFLATED

ROOT = Path(__file__).resolve().parents[1]
version = ElementTree.parse(ROOT / 'Umbra.HuntHelperEvolved.csproj').findtext('./PropertyGroup/Version')
assert version and all(part.isdigit() for part in version.split('.'))
files = {name: ROOT / 'bin/Release' / name
         for name in ('Umbra.HuntHelperEvolved.dll', 'Umbra.HuntHelperEvolved.deps.json')}
files.update({name: ROOT / name for name in ('README.md', 'SOURCES.md', 'VALIDATION.md', 'LICENSE', 'CONTRACT-LICENSE.txt')})
output = ROOT / 'artifacts'
output.mkdir(exist_ok=True)
target = output / f'Umbra.HuntHelperEvolved-{version}.zip'
with ZipFile(target, 'w', compression=ZIP_DEFLATED) as archive:
    for entry, path in files.items():
        data = path.read_bytes()
        for marker in (b'/home/', b'/Users/', b'\\Users\\', b'/tmp/'):
            if marker.lower() in data.lower() or marker.decode().encode('utf-16le').lower() in data.lower():
                raise ValueError(f'Local path marker in {entry}')
        info = ZipInfo(entry, date_time=(2026, 9, 24, 0, 0, 0))
        info.compress_type = ZIP_DEFLATED
        info.external_attr = 0o100644 << 16
        archive.writestr(info, data)
with ZipFile(target) as archive:
    assert archive.testzip() is None
    assert set(archive.namelist()) == set(files)
    assert [name for name in archive.namelist() if name.endswith('.dll')] == ['Umbra.HuntHelperEvolved.dll']
digest = hashlib.sha256(target.read_bytes()).hexdigest()
target.with_suffix('.sha256').write_text(f'{digest}  {target.name}\n', encoding='utf-8')
print(f'{target.name}: {digest}')
