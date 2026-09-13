import { Box, Container, Paper, Typography, TextField, Button, InputAdornment, CssBaseline } from '@mui/material'
import { useState } from 'react'

function App() {
  const [number, setNumber] = useState("")
  const [result, setResult] = useState("")
  const [error, setError] = useState("")
  const [isLoading, setIsLoading] = useState(false)

  async function handleConvert() {
    setError("")
    setResult("")

    const input = number.trim()

    const numberRegex = /^-?(?:[0-9]+(?:\.[0-9]{1,2}0*)?|\.[0-9]{1,2}0*)$/
    if (!numberRegex.test(input)) {
      setError("Enter a valid number with up to two decimal places.")
      return
    }
    if (input.startsWith("-") || !/[1-9]/.test(input)) {
      setError("The number must be greater than zero.")
      return
    }

    // Format the text as a JSON number.
    const parts = input.split(".")
    const integerPart = parts[0].replace(/^0+/, "") || "0"
    let formattedNumber = integerPart
    if (parts.length === 2) {
      formattedNumber += "." + parts[1]
    }

    const maxIntegerPart = "18446744073709551615"
    if (integerPart.length > maxIntegerPart.length || ( integerPart.length === maxIntegerPart.length && integerPart > maxIntegerPart)) {
      setError("The number is too big.")
      return
    }

    setIsLoading(true)
    try {
      const response = await fetch('https://localhost:5001/api/convert', {
        method: 'POST',
        headers: {'Content-Type': 'application/json',},
        body: `{"number":${formattedNumber}}`,
      })

      if (!response.ok) {
        if (response.status === 400) {
          let message = "The number could not be converted."

          try {
            const body = await response.json()

            if (typeof body?.error === "string") {
              message = body.error
            }
          } catch {
            // Some invalid requests return an empty or non-JSON response.
            // In that case nothing happens and we fall back on the default error message;
          }
          
          setError(message)
        }
        else {
          setError("Server could not complete conversion. Please try again.")
        }
        return
      }

      const words = await response.json()
      setResult(words)

    } catch {
      setError("Could not connect to the server. Please try again later.")
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <>
      <CssBaseline />
      <Box
        component="main"
        sx={{ minHeight: '100dvh', bgcolor: { xs: 'background.paper', sm: 'grey.100' },  
              display: 'flex', alignItems: { xs: 'stretch', sm: 'center' }, py: { xs: 0, sm: 4 },}}
      >
        <Container maxWidth="sm" disableGutters sx={{ display: 'flex', px: { xs: 0, sm: 3 }, }}>
          <Paper variant="outlined" 
                sx={{ p: { xs: 3, sm: 4 }, borderRadius: { xs: 0, sm: 3 }, width: '100%', minHeight: { xs: '100dvh', sm: 'auto' },
                      border: { xs: 'none', sm: '1px solid' }, borderColor: 'divider',}}
          >
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
                  onKeyDown={(event) => {
                    if (event.key === "Enter" && number.trim() !== "" && !isLoading ) 
                    {
                      event.preventDefault()
                      handleConvert()
                    }
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

                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2, mt: 2 }}>
                  <Button variant="contained" disabled={number.trim() === "" || isLoading} onClick={handleConvert} sx={{ px: 4 }}>
                    {isLoading ? "Converting..." : "Convert to words"}
                  </Button>

                  <Button variant="outlined" color="inherit" disabled={isLoading} sx={{ boxShadow: 1 }} onClick={() => {
                      setNumber("")
                      setResult("")
                      setError("")
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