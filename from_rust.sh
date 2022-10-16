#!/bin/sh

# Clean
rm -rf src/ICAgentFFI/ic-agent-ffi.dylib
dotnet clean

# Compile
cd ~/agent-unity/ic-agent-ffi
cargo rustc --release -- --crate-type=cdylib

# Copy
mv target/release/libic_agent_ffi.dylib ~/agent-unity-laboratory/src/ICAgentFFI/ic-agent-ffi.dylib