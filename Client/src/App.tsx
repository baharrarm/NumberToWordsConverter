import { Box, Container, Paper, Typography, TextField, Button, InputAdornment } from '@mui/material'
import { useState } from 'react'

function App() {
  const [number, setNumber] = useState('')
  const [result, setResult] = useState('')
  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(false)

  async function handleConvert() {
    setError('')
    setResult('')

    const input = number.trim()

    const numberRegex = /^-?(?:[0-9]+(?:\.[0-9]{1,2}0*)?|\.[0-9]{1,2}0*)$/
    if (!numberRegex.test(input)) {
      setError('Enter a valid number with up to two decimal places.')
      return
    }
    if (parseFloat(input) <= 0) {
      setError('The number must be greater than zero.')
      return
    }

    setIsLoading(true)
    try {
      const response = await fetch('https://localhost:5001/api/convert', {
        method: 'POST',
        headers: {'Content-Type': 'application/json',},
        body: JSON.stringify({ number: input }),
      })

      if (!response.ok) {
        if (response.status === 400) {
          const body = await response.json()
          if (body.error !== null && body.error !== undefined) {
            setError(body.error)
          } 
          else {
            setError('The number could not be converted.')
          }
        } 
        else {
          setError('Server could not complete conversion. Please try again.')
        }
        return
      }

      const words = await response.json()
      setResult(words)

    } catch {
      setError('Could not connect to the API. Check that it is running.')
    } finally {
      setIsLoading(false)
    }
  }

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
                  onChange={(event) => {
                    setNumber(event.target.value)
                    setError("")
                    setResult("")
                  }}
                  disabled={isLoading}
                  error={error !== ""}
                  size="small"
                  fullWidth
                  helperText={error || "Enter a positive amount with up to 2 decimal places."}
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
                  <Button variant="contained" disabled={number.trim() === '' || isLoading} onClick={handleConvert} sx={{ px: 4 }}>
                    {isLoading ? 'Converting...' : 'Convert to words'}
                  </Button>

                  <Button color="inherit" disabled={isLoading} onClick={() => {
                      setNumber('')
                      setResult('')
                      setError('')
                    }}>
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
                    {result}
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