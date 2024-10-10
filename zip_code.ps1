# Nome do arquivo zip final
$zipName = "visionserver_clean.zip"

# Diretório raiz do projeto (modifique se necessário)
$sourceDir = "."

# Lista de padrões de caminhos a serem excluídos (pasta e seus subdiretórios)
$excludes = @(
    "\.git\", 
    "\.vs\", 
    "front\exclusiva-vision\.angular\", 
    "front\exclusiva-vision\node_modules\", 
    "front\exclusiva-vision\www\", 
    "server\.vs\", 
    "server\Vision.Data\bin\", 
    "server\Vision.Data\obj\", 
    "server\VisionAPI\bin\", 
    "server\VisionAPI\obj\",
    "server\VisionOrchestrator\bin\", 
    "server\VisionOrchestrator\obj\"
)

# Função para obter arquivos que serão incluídos e excluídos
function Get-ZipEntry {
    param([string]$path)
    $includedFiles = @()
    $excludedFiles = @()

    # Normalizar o caminho raiz para evitar problemas de comparação
    $normalizedRoot = (Get-Item $path).FullName.Replace('/', '\').ToLower()

    if (Test-Path $path -PathType Container) {
        $allFiles = Get-ChildItem $path -Recurse -File
        $totalFiles = $allFiles.Count
        Write-Host "Total de arquivos encontrados: $totalFiles"

        $counter = 0
        foreach ($file in $allFiles) {
            $counter++
            if ($counter % 100 -eq 0) {
                Write-Host "`rProcessando arquivo $counter de $totalFiles..." -NoNewline
            }

            # Normalizar o caminho para comparação
            $normalizedPath = $file.FullName.Replace('/', '\').ToLower()

            # Verificar se o caminho do arquivo contém qualquer uma das palavras na lista de exclusão
            $isExcluded = $excludes | ForEach-Object {
                $normalizedExclude = $_.Replace('/', '\').ToLower()
                if ($normalizedPath -like "*$normalizedExclude*") {
                    $true
                }
            }

            if ($isExcluded) {
                $excludedFiles += $file
            } else {
                # Calcular o caminho relativo em relação ao diretório raiz normalizado
                $relativePath = $file.FullName.Substring($normalizedRoot.Length).TrimStart('\')
                $includedFiles += New-Object psobject -Property @{
                    FullPath = $file.FullName
                    RelativePath = $relativePath
                }
            }
        }
    }

    return @{
        IncludedCount = $includedFiles.Count
        ExcludedCount = $excludedFiles.Count
        IncludedFiles = $includedFiles
    }
}

# Coletar os arquivos que serão incluídos e excluídos
Write-Host "Preparando para criar o arquivo zip: $zipName"
$zipEntries = Get-ZipEntry $sourceDir
$filesToZip = $zipEntries.IncludedFiles
$includedCount = $zipEntries.IncludedCount
$excludedCount = $zipEntries.ExcludedCount

# Exibir contagem dos arquivos processados
Write-Host "`nArquivos incluídos no zip: $includedCount"
Write-Host "Arquivos ignorados: $excludedCount"

# Criar o arquivo zip mantendo a estrutura de pastas
Write-Host "`nIniciando a criação do zip..."
Add-Type -AssemblyName System.IO.Compression.FileSystem

# Criar o arquivo ZIP manualmente para preservar os caminhos relativos
$zipFileStream = [System.IO.File]::Open($zipName, [System.IO.FileMode]::Create)
$zip = New-Object System.IO.Compression.ZipArchive($zipFileStream, [System.IO.Compression.ZipArchiveMode]::Create)

foreach ($file in $filesToZip) {
    $entry = $zip.CreateEntry($file.RelativePath)

    $entryStream = $entry.Open()
    $fileStream = [System.IO.File]::OpenRead($file.FullPath)
    $fileStream.CopyTo($entryStream)

    $fileStream.Close()
    $entryStream.Close()
}

$zip.Dispose()
$zipFileStream.Close()

Write-Host "`nArquivo $zipName criado com sucesso"
