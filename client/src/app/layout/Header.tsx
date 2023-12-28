import { AppBar, Switch, Toolbar, Typography } from '@mui/material';

interface Props {
  darkMode: boolean;
  handleChangeMode: () => void;
}
export default function Header({ darkMode, handleChangeMode }: Props) {
  return (
    <AppBar position="static">
      <Toolbar>
        <Typography variant="h6">RE-STORE</Typography>
        <Switch checked={darkMode} onChange={handleChangeMode} />
      </Toolbar>
    </AppBar>
  );
}
