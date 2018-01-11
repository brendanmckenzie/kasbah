import React from 'react'
import PropTypes from 'prop-types'
import { Route, Switch } from 'react-router'
import ContentTree from 'components/ContentTree'
import ContentDetail from './ContentDetail'
import CreateButton from './CreateButton'
import { Section, Container, Columns, Column, Field } from 'components/Layout'

const renderBrowser = ({ match }) => <ContentTree context='browser' selected={match && match.params.id} />

renderBrowser.propTypes = {
  match: PropTypes.object
}

const View = () => (
  <Section>
    <Container>
      <Columns>
        <Column className='is-narrow'>
          <Field>
            <Switch>
              <Route exact path='/content' render={renderBrowser} />
              <Route exact path='/content/:id' render={renderBrowser} />
            </Switch>
          </Field>
          <Field>
            <CreateButton className='button is-primary is-fullwidth is-small' />
          </Field>
        </Column>
        <Column>
          <Route exact path='/content/:id' component={ContentDetail} />
        </Column>
      </Columns>
    </Container>
  </Section>
)

export default View
