import React from 'react'
import Information from './Information'
import Analytics from './Analytics'
import { Tabs, Tab } from 'components/PanelTabs'

export const SideBar = () => (
  <Tabs>
    <Tab title='Information'>
      <Information />
    </Tab>
    <Tab title='Analytics'>
      <Analytics />
    </Tab>
  </Tabs>
)

export default SideBar
