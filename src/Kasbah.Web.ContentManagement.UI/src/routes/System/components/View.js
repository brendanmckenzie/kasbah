import React from 'react'
import { Route, Switch } from 'react-router'
import Navigation from './Navigation'
import Sites from './Sites'
import { Section, Container, Columns, Column } from 'components/Layout'

export const View = () => (
  <Section>
    <Container>
      <h1 className='title'>System</h1>
      <Columns>
        <Column className='is-2'>
          <Navigation />
        </Column>
        <Column>
          <Switch>
            <Route exact path='/system/sites' component={Sites} />
          </Switch>
        </Column>
      </Columns>
    </Container>
  </Section>
)

export default View
