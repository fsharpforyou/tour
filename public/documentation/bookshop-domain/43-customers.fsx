type CustomerId = CustomerId of int
type EmailAddress = EmailAddress of string

type Membership =
    | Standard
    | Member of discountPercent: int

type Customer = {
    Id: CustomerId
    Name: string
    Email: EmailAddress option
    Membership: Membership
}

type EmailError = InvalidEmail

let createEmail (text: string) =
    let cleaned = text.Trim()

    if cleaned.Contains("@") then
        Ok(EmailAddress cleaned)
    else
        Error InvalidEmail

let discountPercent customer =
    match customer.Membership with
    | Standard -> 0
    | Member percent -> percent

let customer = {
    Id = CustomerId 1
    Name = "Ada"
    Email = Some(EmailAddress "ada@example.org")
    Membership = Member 10
}

printfn "%s receives a %d%% discount" customer.Name (discountPercent customer)

// Try Standard membership, then create an invalid email through createEmail.
