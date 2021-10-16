# Imports
import argparse
import os

# Global Values
from python.FileOperations import write_buffer_to_file

read_size = 0x10
starting_bytes = b'\x00\x00\x01\xBA\x44'
ending_bytes = b'\x00\x00\x01\xB9\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00'


def extract_file_content(video_file_info, file_read_stream):
    file_size = video_file_info['end'] - video_file_info['start']

    if file_size <= 0:
        return

    file_read_stream.seek(video_file_info['start'])

    full_buffer = file_read_stream.read(file_size)

    return full_buffer


def extract_files_save_to_file(video_files_found, filename: str):
    with open(filename, 'rb') as binary_file:
        for video_file in video_files_found:
            video_content = extract_file_content(video_file, binary_file)
            write_buffer_to_file(video_content, os.path.dirname(filename), f'{video_file["index"]}.pss')


def scan_file_for_pss(filename: str):
    files_found = []

    with open(filename, 'rb') as binary_file:
        file_index = 0

        current_bytes = binary_file.read(read_size)
        file_found = False
        file_start = 0x0
        while current_bytes:
            curr_position = binary_file.tell()
            if current_bytes.startswith(starting_bytes) and (curr_position - read_size) % 0x800 == 0 and not file_found:
                file_start = curr_position - read_size
                file_found = True
            if current_bytes.find(ending_bytes) >= 0 and file_found:
                file_found = False

                files_found.append({
                    'index': file_index,
                    'start': file_start,
                    'end': curr_position - 0xC
                })

                print(f'File {file_index} found @{hex(file_start)} -> @{hex(curr_position - 0xC)}')

                file_index += 1

            # Since we know all files start at the beginning of a sector (x * 0x800), we can speed up search
            binary_file.seek(0x7F0, 1)
            current_bytes = binary_file.read(read_size)

    return files_found


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Finds PSS files in a bin file.')
    parser.add_argument('--file', type=str, required=True, help='Binary file that will be scanned for PSS files.')

    args = parser.parse_args()

    video_files = scan_file_for_pss(args.file)

    extract_files_save_to_file(video_files, args.file)
