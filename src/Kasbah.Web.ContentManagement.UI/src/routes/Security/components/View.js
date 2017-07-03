import React from 'react'
import UserList from './UserList'
import { Section, Container } from 'components/Layout'

export const View = () => (
  <Section>
    <Container>
      <h1 className='title'>Security</h1>
      <UserList />
    </Container>
  </Section>
)

export default View
