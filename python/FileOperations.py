# Import statements
from pathlib import Path
import shutil


def write_buffer_to_file(buffer_to_write, output_folder, output_file, mode='wb'):
    recursive_create_folder(output_folder)

    output_file = open(f'{output_folder}/{output_file}', mode)
    output_file.write(buffer_to_write)
    output_file.close()


def recursive_create_folder(folder_path_to_create):
    Path(folder_path_to_create).mkdir(parents=True, exist_ok=True)


def move_and_rename_file(original_file_path_and_name, target_file_path_and_name):
    shutil.move(original_file_path_and_name, target_file_path_and_name)
