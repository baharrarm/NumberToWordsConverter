import { Box, Container, Paper, Typography, TextField, Button, InputAdornment } from '@mui/material'
import { useState } from 'react'

function App() {
  const [number, setNumber] = useState('')

  return (
    <>
      <Box
        component="main"
        sx={{ minHeight: '100vh', bgcolor: 'grey.100', display: 'flex', alignItems: 'center', }}
      >
        <Container maxWidth="sm" >
          <Paper variant="outlined" sx={{ p: 4, borderRadius: 3 }}>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
              <Box>
                <Typography variant="h4" gutterBottom>
                  Number to Words
                </Typography>

                <Typography color="text.secondary">
                  Convert a number into words.
                </Typography>
              </Box>

              <Box>
                <Typography component="label" htmlFor="number" variant="body2" sx={{ mb: 0.5 }} >
                  Number
                </Typography>

                <TextField
                  id="number"
                  placeholder="123.45"
                  value={number}
                  onChange={(event) => setNumber(event.target.value)}
                  size="small"
                  fullWidth
                  helperText="Enter a positive amount with up to 2 decimal places."
                  slotProps={{
                    input: {
                      startAdornment: (
                        <InputAdornment position="start">
                          $
                        </InputAdornment>
                      ),
                    },
                    formHelperText: { sx: { mx: 0 } },
                  }}
                />

                <Box sx={{ display: 'flex', gap: 2, mt: 2 }}>
                  <Button variant="contained" disabled={number.trim() === ''} sx={{ px: 4 }}>
                    Convert to words
                  </Button>

                  <Button color="inherit" onClick={() => setNumber('')}>
                    Clear
                  </Button>
                </Box>
              </Box>

              <Box>
                <Typography variant="subtitle1" gutterBottom>
                  Result
                </Typography>

                <Box sx={{ p: 2, bgcolor: 'grey.100', borderRadius: 1, overflowWrap: 'anywhere', }} >
                  <Typography color="text.secondary">
                    Converted Number will appear here.
                  </Typography>
                </Box>
              </Box>
            </Box>
          </Paper>
        </Container>
      </Box>
    </>
  )
}

export default App