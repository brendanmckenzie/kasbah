import React from 'react'
import Information from './Information'
import Analytics from './Analytics'
import { Tabs, Tab } from 'components/PanelTabs'

export const SideBar = ({ node, type, data }) => (
  <Tabs>
    <Tab title='Information'>
      <Information node={node} type={type} data={data} />
    </Tab>
    <Tab title='Analytics'>
      <Analytics />
    </Tab>
  </Tabs>
)

SideBar.propTypes = {
  node: React.PropTypes.object.isRequired,
  type: React.PropTypes.object.isRequired,
  data: React.PropTypes.object.isRequired
}

export default SideBar
